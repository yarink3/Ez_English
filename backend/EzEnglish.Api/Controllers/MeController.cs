using EzEnglish.Api.Application.Services;
using EzEnglish.Api.Contracts;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController(
    ICurrentParentAccessor currentParent,
    EzEnglishDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<MeResponse>> Get(CancellationToken ct)
    {
        var parent = await currentParent.GetOrCreateAsync(ct);

        var children = await db.Children
            .AsNoTracking()
            .Where(c => c.ParentId == parent.Id)
            .Include(c => c.CategoryLevels)
            .OrderBy(c => c.CreatedAtUtc)
            .ToListAsync(ct);

        var childDtos = children.Select(c => new ChildDto(
            c.Id,
            c.DisplayName,
            c.BirthDate,
            c.CharacterId,
            c.CategoryLevels.Select(l => new ChildCategoryLevelDto(l.Category, l.Level)).ToList())
        ).ToList();

        return Ok(new MeResponse(
            new ParentDto(parent.Id, parent.Email, parent.DisplayName),
            childDtos));
    }
}
