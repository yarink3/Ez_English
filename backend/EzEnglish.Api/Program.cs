using EzEnglish.Api.Application.Authorization;
using EzEnglish.Api.Application.Services;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

// ---------------- Authentication (Firebase ID tokens via JwtBearer) ----------------
// Firebase ID tokens are RS256 JWTs issued by Google Identity Platform.
// JwtBearer fetches Google's public keys from OIDC discovery at
//   https://securetoken.google.com/<projectId>/.well-known/openid-configuration
// so no Firebase service-account JSON is needed on the server.
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"]
    ?? throw new InvalidOperationException("Missing Firebase:ProjectId in configuration.");
var firebaseIssuer = $"https://securetoken.google.com/{firebaseProjectId}";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = firebaseIssuer;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = firebaseIssuer,
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
        };
        // Keep Firebase's original claim names (e.g. "user_id", "email", "name")
        // available alongside the .NET ClaimTypes.* mappings.
        opts.MapInboundClaims = true;
    });

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

