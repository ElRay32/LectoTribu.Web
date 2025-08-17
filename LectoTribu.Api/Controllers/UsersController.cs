using LectoTribu.Domain.Entities;
using LectoTribu.Domain.ValueObjects;
using LectoTribu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    public record CreateUserDto(string FirstName, string LastName, string? Email);
    public record UserListItem(Guid Id, string Name);

    [HttpPost]
    public async Task<ActionResult<UserListItem>> Create([FromBody] CreateUserDto dto, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var name = ($"{dto.FirstName} {dto.LastName}").Trim();
        var email = string.IsNullOrWhiteSpace(dto.Email) ? $"{Guid.NewGuid():N}@example.local" : dto.Email!;
        var user = new User(name, new Email(email));
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, new UserListItem(user.Id, user.Name));
    }

    [HttpGet]
    public async Task<IEnumerable<UserListItem>> List([FromServices] AppDbContext db, CancellationToken ct)
        => await db.Users.OrderBy(u => u.Name).Select(u => new UserListItem(u.Id, u.Name)).ToListAsync(ct);

    [HttpGet("{id}")]
    public async Task<ActionResult<UserListItem>> Get(Guid id, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var u = await db.Users.FindAsync(new object[] { id }, ct);
        return u is null ? NotFound() : new UserListItem(u.Id, u.Name);
    }
}