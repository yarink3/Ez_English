using EzEnglish.Api.Domain.Entities;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Application.Services;

/// <summary>
/// Resolves the authenticated Firebase principal to a <see cref="Parent"/> row,
/// auto-creating one on first login. Cached per-request via DI scope.
/// </summary>
public interface ICurrentParentAccessor
{
    Task<Parent> GetOrCreateAsync(CancellationToken ct);
}

public sealed class CurrentParentAccessor(
    IHttpContextAccessor httpContextAccessor,
    EzEnglishDbContext db,
    ILogger<CurrentParentAccessor> logger) : ICurrentParentAccessor
{
    private Parent? _cached;

    public async Task<Parent> GetOrCreateAsync(CancellationToken ct)
    {
        if (_cached is not null) return _cached;

        var user = httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("No HttpContext available.");

        // Firebase ID tokens carry the user's UID in the "user_id" claim (also mirrored in "sub").
        var uid = user.FindFirst("user_id")?.Value
            ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Missing 'user_id' claim in Firebase token.");

        var email = user.FindFirst("email")?.Value
            ?? user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedAccessException("Missing email claim in Firebase token.");

        var displayName = user.FindFirst("name")?.Value
            ?? user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var parent = await db.Parents.FirstOrDefaultAsync(p => p.FirebaseUid == uid, ct);
        if (parent is null)
        {
            parent = new Parent
            {
                FirebaseUid = uid,
                Email = email,
                DisplayName = displayName,
            };
            db.Parents.Add(parent);
            await db.SaveChangesAsync(ct);
            logger.LogInformation(
                "Auto-provisioned Parent {ParentId} for Firebase uid {Uid}", parent.Id, uid);
        }
        else if (parent.Email != email || (displayName is not null && parent.DisplayName != displayName))
        {
            parent.Email = email;
            if (displayName is not null) parent.DisplayName = displayName;
            await db.SaveChangesAsync(ct);
        }

        _cached = parent;
        return parent;
    }
}

