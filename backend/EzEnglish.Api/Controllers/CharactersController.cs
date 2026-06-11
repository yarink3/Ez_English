using EzEnglish.Api.Contracts;
using EzEnglish.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class CharactersController(EzEnglishDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> List(CancellationToken ct)
    {
        var list = await db.Characters
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CharacterDto(c.Id, c.Key, c.DisplayNameEn, c.DisplayNameHe, c.AvatarUrl))
            .ToListAsync(ct);
        return Ok(list);
    }
}
