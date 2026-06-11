using EzEnglish.Api.Application.Authorization;
using EzEnglish.Api.Application.Services;
using EzEnglish.Api.Infrastructure.Auth;
using EzEnglish.Api.Infrastructure.Persistence;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Logging (Serilog) ----------------
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// ---------------- EF Core ----------------
builder.Services.AddDbContext<EzEnglishDbContext>(opts =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection.");
    opts.UseSqlServer(conn);
});

// ---------------- Firebase Admin SDK init ----------------
// Initialise once per process. Credentials come from either a service-account
// JSON file (Firebase:ServiceAccountPath in appsettings) or the standard
// GOOGLE_APPLICATION_CREDENTIALS env var. ProjectId is taken from config.
// If no credentials are available we skip init — token validation will fail
// loudly per-request, but the host can still start (so EF design-time tools
// and Swagger generation keep working in environments without secrets).
var firebaseSection = builder.Configuration.GetSection("Firebase");
var firebaseProjectId = firebaseSection["ProjectId"]
    ?? throw new InvalidOperationException("Missing Firebase:ProjectId in configuration.");
if (FirebaseApp.DefaultInstance is null)
{
    var serviceAccountPath = firebaseSection["ServiceAccountPath"];
    GoogleCredential? credential = null;
    if (!string.IsNullOrWhiteSpace(serviceAccountPath) && File.Exists(serviceAccountPath))
    {
        credential = GoogleCredential.FromFile(serviceAccountPath);
    }
    else
    {
        try { credential = GoogleCredential.GetApplicationDefault(); }
        catch { /* No ADC available — handled below. */ }
    }
    if (credential is not null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = credential,
            ProjectId = firebaseProjectId,
        });
    }
}

// ---------------- Authentication (Firebase) ----------------
// Custom JwtBearer-style scheme that delegates token validation to the
// Firebase Admin SDK. See Infrastructure/Auth/FirebaseAuthenticationHandler.cs.
builder.Services
    .AddAuthentication(FirebaseAuthOptions.SchemeName)
    .AddScheme<FirebaseAuthOptions, FirebaseAuthenticationHandler>(
        FirebaseAuthOptions.SchemeName,
        opts => opts.ProjectId = firebaseProjectId);

// ---------------- Authorization ----------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ChildOwnership.PolicyName, p =>
    {
        p.RequireAuthenticatedUser();
        p.Requirements.Add(new ChildOwnedByCurrentParentRequirement());
    });
});
builder.Services.AddScoped<IAuthorizationHandler, ChildOwnedByCurrentParentHandler>();

// ---------------- Application services ----------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentParentAccessor, CurrentParentAccessor>();

// ---------------- CORS ----------------
const string CorsPolicyName = "FrontendCors";
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:5173"];
    options.AddPolicy(CorsPolicyName, p => p
        .WithOrigins(origins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ---------------- MVC + Swagger ----------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ez English API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste a Firebase ID token here.",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

