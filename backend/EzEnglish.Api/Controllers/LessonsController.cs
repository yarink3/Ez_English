using System.Text.Json;
using EzEnglish.Api.Application.Authorization;
using EzEnglish.Api.Contracts;
using EzEnglish.Api.Domain.Enums;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Controllers;

/// <summary>
/// Lesson catalogue scoped to a specific child. The child-ownership policy
/// guarantees the caller is the parent of the child in the route.
/// </summary>
[ApiController]
[Route("api/children/{childId:int}/lessons")]
[Authorize(Policy = ChildOwnership.PolicyName)]
public sealed class LessonsController(EzEnglishDbContext db) : ControllerBase
{
    /// <summary>
    /// Lessons available for the given child in <paramref name="category"/>.
    /// Includes anything at or below the child's current level for that category.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonSummaryDto>>> List(
        [FromRoute] int childId,
        [FromQuery] Category category,
        CancellationToken ct)
    {
        var ccl = await db.ChildCategoryLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChildId == childId && x.Category == category, ct);

        if (ccl is null)
            return Ok(Array.Empty<LessonSummaryDto>());

        var lessons = await db.Lessons
            .AsNoTracking()
            .Where(l => l.Category == category && l.Level <= ccl.Level)
            .OrderBy(l => l.Level)
            .ThenBy(l => l.OrderInLevel)
            .Select(l => new LessonSummaryDto(
                l.Id, l.Category, l.Level, l.TitleEn, l.TitleHe, l.OrderInLevel,
                l.Items.Count))
            .ToListAsync(ct);

        return Ok(lessons);
    }

    /// <summary>Full lesson with items. 403 if the lesson is above the child's level.</summary>
    [HttpGet("{lessonId:int}")]
    public async Task<ActionResult<LessonDetailDto>> Get(
        [FromRoute] int childId,
        [FromRoute] int lessonId,
        CancellationToken ct)
    {
        var lesson = await db.Lessons
            .AsNoTracking()
            .Include(l => l.Items.OrderBy(i => i.OrderInLesson))
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct);

        if (lesson is null) return NotFound();

        var ccl = await db.ChildCategoryLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChildId == childId && x.Category == lesson.Category, ct);

        if (ccl is null || lesson.Level > ccl.Level)
            return Forbid();

        var dto = new LessonDetailDto(
            lesson.Id, lesson.Category, lesson.Level, lesson.TitleEn, lesson.TitleHe,
            lesson.Items.Select(i => new LessonItemDto(
                i.Id, i.Kind, i.OrderInLesson, i.PromptEn, i.PromptHe,
                ParsePayload(i.PayloadJson))).ToList());

        return Ok(dto);
    }

    private static JsonElement ParsePayload(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}
