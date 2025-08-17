using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LectoTribu.Application.DTOs;
using LectoTribu.Application.Interfaces;
using LectoTribu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController : ControllerBase
{
    private readonly IClubService _service;
    public ClubsController(IClubService service) => _service = service;

    // ===== DTOs locales =====
    public record ClubListItem(Guid Id, string Name, string? Description);
    public record UpdateClubDto(string Name, string? Description);

    // ===== CRUD =====
    [HttpGet]
    public async Task<IEnumerable<ClubListItem>> List([FromServices] AppDbContext db, CancellationToken ct)
        => await db.Clubs.OrderBy(c => c.Name)
              .Select(c => new ClubListItem(c.Id, c.Name, c.Description))
              .ToListAsync(ct);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClubDto dto, CancellationToken ct)
    {
        var club = await _service.CreateClubAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = club.Id }, new { club.Id, club.Name });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClubListItem>> GetById(Guid id, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var c = await db.Clubs.Where(x => x.Id == id)
                 .Select(x => new ClubListItem(x.Id, x.Name, x.Description))
                 .FirstOrDefaultAsync(ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClubDto dto,
        [FromServices] AppDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");
        var club = await db.Clubs.FindAsync(new object[] { id }, ct);
        if (club is null) return NotFound("Club no existe");
        club.UpdateDetails(dto.Name, dto.Description);
        db.Update(club);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var club = await db.Clubs.FindAsync(new object[] { id }, ct);
        if (club is null) return NotFound();
        db.Remove(club);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ===== Libros / Miembros =====
    [HttpPost("{id}/books")]
    public async Task<IActionResult> AddBook(Guid id, [FromBody] AddBookDto dto, CancellationToken ct)
    {
        if (dto.BookId == Guid.Empty) return BadRequest("BookId is required.");
        await _service.AddBookAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpPost("{id}/members/by-email")]
    public async Task<IActionResult> AddMemberByEmail(Guid id, [FromBody] AddMemberByEmailDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email is required.");
        await _service.AddMemberAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    // ===== Programación =====
    [HttpPost("{id}/schedule/one")]
    public async Task<IActionResult> ScheduleOne(
        Guid id,
        [FromBody] ScheduleChapterDto dto,
        [FromServices] AppDbContext db,
        CancellationToken ct)
    {
        if (id == Guid.Empty) return BadRequest("ClubId is required.");
        if (dto.BookId == Guid.Empty) return BadRequest("BookId is required.");
        if (dto.Chapter <= 0) return BadRequest("Chapter must be > 0.");
        if (dto.Date == default) return BadRequest("Date is required.");

        var exists = await db.Clubs.AnyAsync(c => c.Id == id, ct);
        if (!exists) return NotFound("Club no existe");

        await _service.ScheduleAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpPost("{id}/schedule/plan")]
    public async Task<IActionResult> SchedulePlan(
        Guid id,
        [FromBody] SchedulePlanDto dto,
        [FromServices] AppDbContext db,
        CancellationToken ct)
    {
        if (id == Guid.Empty) return BadRequest("ClubId is required.");
        if (dto.BookId == Guid.Empty) return BadRequest("BookId is required.");
        if (dto.ChaptersPerWeek <= 0) return BadRequest("ChaptersPerWeek must be > 0.");
        if (dto.TotalChapters <= 0) return BadRequest("TotalChapters must be > 0.");
        if (dto.StartDate == default) return BadRequest("StartDate is required.");

        var exists = await db.Clubs.AnyAsync(c => c.Id == id, ct);
        if (!exists) return NotFound("Club no existe");

        await _service.ScheduleAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    // ===== Comentarios =====
    [HttpPost("{id}/comment")]
    public async Task<IActionResult> Comment(Guid id, [FromBody] CommentDto dto, CancellationToken ct)
    {
        if (dto.UserId == Guid.Empty) return BadRequest("UserId is required.");
        if (dto.BookId == Guid.Empty) return BadRequest("BookId is required.");
        if (dto.Chapter <= 0) return BadRequest("Chapter must be > 0.");
        if (string.IsNullOrWhiteSpace(dto.Content)) return BadRequest("Content is required.");

        await _service.CommentAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    // Retorna con nombre del usuario
    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDtoOut>>> GetComments(
        Guid id, [FromQuery] Guid bookId, [FromQuery] int chapter,
        [FromServices] AppDbContext db, CancellationToken ct)
    {
        var query =
            from c in db.Comments
            join u in db.Users on c.UserId equals u.Id
            where c.ClubId == id && c.BookId == bookId && c.ChapterNumber == chapter
            orderby c.CreatedAtUtc
            select new CommentDtoOut(c.UserId, u.Name, c.Content, c.CreatedAtUtc);

        var list = await query.ToListAsync(ct);
        return Ok(list);
    }
}

// Evita que se descubra como controller:
[NonController]
public record CommentDtoOut(Guid UserId, string UserName, string Content, DateTime CreatedAtUtc);
