using System.Security.Claims;
using System.Text.Encodings.Web;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EzEnglish.Api.Infrastructure.Auth;

/// <summary>
/// AuthenticationHandler that validates a Firebase ID token presented in the
/// <c>Authorization: Bearer &lt;token&gt;</c> header using the Firebase Admin SDK
/// and produces a ClaimsPrincipal with the standard Firebase claims (uid, email,
/// name, etc.) preserved under their original claim names.
/// </summary>
public sealed class FirebaseAuthenticationHandler : AuthenticationHandler<FirebaseAuthOptions>
{
    public FirebaseAuthenticationHandler(
        IOptionsMonitor<FirebaseAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var rawHeader))
        {
            return AuthenticateResult.NoResult();
        }

        var header = rawHeader.ToString();
        if (string.IsNullOrWhiteSpace(header) ||
            !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var token = header["Bearer ".Length..].Trim();
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.NoResult();
        }

        FirebaseToken decoded;
        try
        {
            decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, Context.RequestAborted);
        }
        catch (FirebaseAuthException ex)
        {
            Logger.LogWarning(ex, "Firebase ID token verification failed");
            return AuthenticateResult.Fail(ex);
        }

        // Project-id sanity check — VerifyIdTokenAsync already enforces audience,
        // but a misconfigured ProjectId option is worth surfacing loudly.
        if (!string.IsNullOrEmpty(Options.ProjectId) &&
            !string.Equals(decoded.Audience, Options.ProjectId, StringComparison.Ordinal))
        {
            return AuthenticateResult.Fail(
                $"Token audience '{decoded.Audience}' does not match expected project '{Options.ProjectId}'.");
        }

        var claims = new List<Claim>
        {
            new("user_id", decoded.Uid),
            new(ClaimTypes.NameIdentifier, decoded.Uid),
        };

        foreach (var (key, value) in decoded.Claims)
        {
            if (value is null) continue;
            var stringValue = value as string ?? value.ToString();
            if (string.IsNullOrEmpty(stringValue)) continue;
            claims.Add(new Claim(key, stringValue));
            if (key == "email")
            {
                claims.Add(new Claim(ClaimTypes.Email, stringValue));
            }
            else if (key == "name")
            {
                claims.Add(new Claim(ClaimTypes.Name, stringValue));
            }
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name, ClaimTypes.NameIdentifier, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
