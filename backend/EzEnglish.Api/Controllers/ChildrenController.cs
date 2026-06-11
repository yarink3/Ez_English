using EzEnglish.Api.Application.Authorization;
using EzEnglish.Api.Application.Services;
using EzEnglish.Api.Contracts;
using EzEnglish.Api.Domain.Entities;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/children")]
public sealed class ChildrenController(
    EzEnglishDbContext db,
    ICurrentParentAccessor currentParent) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChildDto>>> List(CancellationToken ct)
    {
        var parent = await currentParent.GetOrCreateAsync(ct);
        var children = await db.Children
            .AsNoTracking()
            .Where(c => c.ParentId == parent.Id)
            .Include(c => c.CategoryLevels)
            .OrderBy(c => c.CreatedAtUtc)
            .ToListAsync(ct);

        return Ok(children.Select(c => Map(c)));
    }

    [HttpPost]
    public async Task<ActionResult<ChildDto>> Create(
        [FromBody] CreateChildRequest req,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DisplayName) || req.DisplayName.Length > 64)
            return BadRequest(new { error = "DisplayName must be 1–64 characters." });

        var character = await db.Characters.FindAsync([req.CharacterId], ct);
        if (character is null) return BadRequest(new { error = "Unknown CharacterId." });

        var parent = await currentParent.GetOrCreateAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var ageYears = AgeLevelMapper.AgeYears(req.BirthDate, today);
        var startingLevel = AgeLevelMapper.StartingLevelForAge(ageYears);

        var child = new Child
        {
            ParentId = parent.Id,
            DisplayName = req.DisplayName.Trim(),
            BirthDate = req.BirthDate,
            CharacterId = req.CharacterId,
            PinHash = string.IsNullOrEmpty(req.Pin) ? null : HashPin(req.Pin),
            CategoryLevels = AgeLevelMapper.AllCategories
                .Select(cat => new ChildCategoryLevel { Category = cat, Level = startingLevel })
                .ToList(),
        };

        db.Children.Add(child);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Get), new { childId = child.Id }, Map(child));
    }

    [HttpGet("{childId:int}")]
    [Authorize(Policy = ChildOwnership.PolicyName)]
    public async Task<ActionResult<ChildDto>> Get([FromRoute] int childId, CancellationToken ct)
    {
        var child = await db.Children
            .AsNoTracking()
            .Include(c => c.CategoryLevels)
            .FirstOrDefaultAsync(c => c.Id == childId, ct);
        return child is null ? NotFound() : Ok(Map(child));
    }

    [HttpDelete("{childId:int}")]
    [Authorize(Policy = ChildOwnership.PolicyName)]
    public async Task<IActionResult> Delete([FromRoute] int childId, CancellationToken ct)
    {
        var child = await db.Children.FindAsync([childId], ct);
        if (child is null) return NotFound();
        db.Children.Remove(child);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static ChildDto Map(Child c) => new(
        c.Id, c.DisplayName, c.BirthDate, c.CharacterId,
        c.CategoryLevels.Select(l => new ChildCategoryLevelDto(l.Category, l.Level)).ToList());

    private static readonly PasswordHasher<string> PinHasher = new();
    private static string HashPin(string pin) => PinHasher.HashPassword(string.Empty, pin);
}
