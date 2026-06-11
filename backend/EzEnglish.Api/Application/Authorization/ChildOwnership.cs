using EzEnglish.Api.Application.Services;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Application.Authorization;

/// <summary>
/// Marker requirement; enforced by <see cref="ChildOwnedByCurrentParentHandler"/>.
/// Apply via <c>[Authorize(Policy = ChildOwnership.PolicyName)]</c> on endpoints
/// whose route includes <c>{childId:int}</c>.
/// </summary>
public sealed class ChildOwnedByCurrentParentRequirement : IAuthorizationRequirement
{
    public const string RouteValueName = "childId";
}

public static class ChildOwnership
{
    public const string PolicyName = "ChildOwnedByCurrentParent";
}

public sealed class ChildOwnedByCurrentParentHandler(
    IHttpContextAccessor httpContextAccessor,
    EzEnglishDbContext db,
    ICurrentParentAccessor currentParent,
    ILogger<ChildOwnedByCurrentParentHandler> logger)
    : AuthorizationHandler<ChildOwnedByCurrentParentRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ChildOwnedByCurrentParentRequirement requirement)
    {
        var http = httpContextAccessor.HttpContext;
        if (http is null) return;

        if (!http.Request.RouteValues.TryGetValue(
                ChildOwnedByCurrentParentRequirement.RouteValueName, out var raw)
            || raw is null
            || !int.TryParse(raw.ToString(), out var childId))
        {
            logger.LogDebug("Child ownership policy applied to a route without a childId; skipping.");
            return;
        }

        var parent = await currentParent.GetOrCreateAsync(http.RequestAborted);
        var owned = await db.Children
            .AsNoTracking()
            .AnyAsync(c => c.Id == childId && c.ParentId == parent.Id, http.RequestAborted);

        if (owned)
            context.Succeed(requirement);
        else
            logger.LogWarning(
                "Parent {ParentId} attempted to access Child {ChildId} they do not own.",
                parent.Id, childId);
    }
}
