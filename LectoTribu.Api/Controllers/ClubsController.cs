using LectoTribu.Application.DTOs;
using LectoTribu.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LectoTribu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClubsController : ControllerBase
{
    private readonly IClubService _service;
    public ClubsController(IClubService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClubDto dto, CancellationToken ct)
    {
        var club = await _service.CreateClubAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = club.Id }, new { club.Id, club.Name });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id) => Ok(new { Id = id }); // Placeholder mínimo

    [HttpPost("{id}/books")]
    public async Task<IActionResult> AddBook(Guid id, [FromBody] Guid bookId, CancellationToken ct)
    {
        await _service.AddBookAsync(new AddBookDto(id, bookId), ct);
        return NoContent();
    }

    [HttpPost("{id}/members/by-id")]
    public async Task<IActionResult> AddMemberById(Guid id, [FromBody] Guid userId, CancellationToken ct)
    {
        await _service.AddMemberAsync(id, userId, ct);
        return NoContent();
    }

    [HttpPost("{id}/members/by-email")]
    public async Task<IActionResult> AddMemberByEmail(Guid id, [FromBody] string email, CancellationToken ct)
    {
        await _service.AddMemberAsync(new AddMemberByEmailDto(id, email), ct);
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

    [HttpPost("{id}/comment")]
    public async Task<IActionResult> Comment(Guid id, [FromBody] CommentDto dto, CancellationToken ct)
    {
        await _service.CommentAsync(dto with { ClubId = id }, ct);
        return NoContent();
    }
}
