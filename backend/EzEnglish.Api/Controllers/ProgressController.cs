using EzEnglish.Api.Application.Authorization;
using EzEnglish.Api.Contracts;
using EzEnglish.Api.Domain.Entities;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Controllers;

/// <summary>
/// Records a child's score on a lesson item. Score is 0–100 and idempotent per
/// (child, item) pair: the latest score wins, attempt count is incremented.
/// </summary>
[ApiController]
[Route("api/children/{childId:int}/progress")]
[Authorize(Policy = ChildOwnership.PolicyName)]
public sealed class ProgressController(EzEnglishDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgressDto>>> List(
        [FromRoute] int childId,
        CancellationToken ct)
    {
        var rows = await db.Progress
            .AsNoTracking()
            .Where(p => p.ChildId == childId)
            .Select(p => new ProgressDto(
                p.LessonItemId, p.Score, p.AttemptCount,
                p.FirstAttemptedAtUtc, p.LastAttemptedAtUtc, p.CompletedAtUtc))
            .ToListAsync(ct);

        return Ok(rows);
    }

    [HttpPost]
    public async Task<ActionResult<ProgressDto>> Submit(
        [FromRoute] int childId,
        [FromBody] SubmitProgressRequest req,
        CancellationToken ct)
    {
        if (req.Score < 0 || req.Score > 100)
            return BadRequest(new { error = "Score must be 0–100." });

        var item = await db.LessonItems
            .AsNoTracking()
            .Include(i => i.Lesson)
            .FirstOrDefaultAsync(i => i.Id == req.LessonItemId, ct);
        if (item is null) return BadRequest(new { error = "Unknown LessonItemId." });

        var ccl = await db.ChildCategoryLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ChildId == childId && x.Category == item.Lesson.Category, ct);
        if (ccl is null || item.Lesson.Level > ccl.Level)
            return Forbid();

        var now = DateTime.UtcNow;
        var progress = await db.Progress
            .FirstOrDefaultAsync(
                p => p.ChildId == childId && p.LessonItemId == req.LessonItemId, ct);

        if (progress is null)
        {
            progress = new Progress
            {
                ChildId = childId,
                LessonItemId = req.LessonItemId,
                Score = req.Score,
                AttemptCount = 1,
                FirstAttemptedAtUtc = now,
                LastAttemptedAtUtc = now,
                CompletedAtUtc = req.Score >= 80 ? now : null,
            };
            db.Progress.Add(progress);
        }
        else
        {
            progress.Score = req.Score;
            progress.AttemptCount++;
            progress.LastAttemptedAtUtc = now;
            if (req.Score >= 80 && progress.CompletedAtUtc is null)
                progress.CompletedAtUtc = now;
        }

        await db.SaveChangesAsync(ct);

        return Ok(new ProgressDto(
            progress.LessonItemId, progress.Score, progress.AttemptCount,
            progress.FirstAttemptedAtUtc, progress.LastAttemptedAtUtc, progress.CompletedAtUtc));
    }
}
