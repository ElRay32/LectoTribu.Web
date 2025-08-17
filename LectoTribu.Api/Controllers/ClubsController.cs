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

    public ClubsController(IClubService service)
    {
        _service = service;
    }

    // ====== CRUD básico de Clubs ======

    public record ClubListItem(Guid Id, string Name, string? Description);
    public record UpdateClubDto(string Name, string? Description);

    [HttpGet]
    public async Task<IEnumerable<ClubListItem>> List([FromServices] AppDbContext db, CancellationToken ct)
        => await db.Clubs
                   .OrderBy(c => c.Name)
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
        var club = await db.Clubs.FindAsync(new object[] { id }, ct);
        if (club is null) return NotFound();

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

    // ====== Gestión de libros / miembros / scheduling ======

    [HttpPost("{id}/books")]
    public async Task<IActionResult> AddBook(Guid id, [FromBody] AddBookDto dto, CancellationToken ct)
    {
        await _service.AddBookAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpPost("{id}/members/by-email")]
    public async Task<IActionResult> AddMemberByEmail(Guid id, [FromBody] AddMemberByEmailDto dto, CancellationToken ct)
    {
        // El DTO trae Email + ClubId, pero sobreescribimos el ClubId por seguridad
        await _service.AddMemberAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpPost("{id}/schedule/one")]
    public async Task<IActionResult> ScheduleOne(Guid id, [FromBody] ScheduleChapterDto dto, CancellationToken ct)
    {
        await _service.ScheduleAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpPost("{id}/schedule/plan")]
    public async Task<IActionResult> SchedulePlan(Guid id, [FromBody] SchedulePlanDto dto, CancellationToken ct)
    {
        await _service.ScheduleAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    // ====== Comentarios ======

    [HttpPost("{id}/comment")]
    public async Task<IActionResult> Comment(Guid id, [FromBody] CommentDto dto, CancellationToken ct)
    {
        await _service.CommentAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDtoOut>>> GetComments(
        Guid id,
        [FromQuery] Guid bookId,
        [FromQuery] int chapter,
        [FromServices] AppDbContext db,
        CancellationToken ct)
    {
        // Proyecta al DTO de salida incluyendo el nombre del usuario
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

// ====== DTOs de salida (fuera del controller) ======
// Muy importante: fuera del controller y marcado como NonController para que nunca se descubra como endpoint.
[NonController]
public record CommentDtoOut(Guid UserId, string UserName, string Content, DateTime CreatedAtUtc);
