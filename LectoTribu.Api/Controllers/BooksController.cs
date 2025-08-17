using LectoTribu.Domain.Entities;
using LectoTribu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    public record CreateBookDto(string Title, string AuthorName, int TotalChapters,
                                string Genre, BookFormat Format,
                                string? Publisher, string? Language, int? Pages, int? Year, string? Isbn);

    public record UpdateBookDto(string Title, string AuthorName, int TotalChapters,
                                string Genre, BookFormat Format,
                                string? Publisher, string? Language, int? Pages, int? Year, string? Isbn);

    public record BookListItem(Guid Id, string Title);

    [HttpGet]
    public async Task<IEnumerable<BookListItem>> List([FromServices] AppDbContext db, CancellationToken ct)
        => await db.Books.OrderBy(b => b.Title).Select(b => new BookListItem(b.Id, b.Title)).ToListAsync(ct);

    [HttpGet("{id}")]
    public async Task<ActionResult<BookListItem>> Get(Guid id, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var b = await db.Books.FindAsync(new object[] { id }, ct);
        return b is null ? NotFound() : new BookListItem(b.Id, b.Title);
    }

    [HttpPost]
    public async Task<ActionResult<BookListItem>> Create([FromBody] CreateBookDto dto, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == dto.AuthorName, ct) ?? new Author(dto.AuthorName);
        var book = new Book(dto.Title, author.Id, dto.TotalChapters,
                            dto.Genre, dto.Format, dto.Publisher, dto.Language, dto.Pages, dto.Year);
        await db.AddRangeAsync(author, book);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = book.Id }, new BookListItem(book.Id, book.Title));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var book = await db.Books.FindAsync(new object[] { id }, ct);
        if (book is null) return NotFound();

        // asegurar autor
        var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == dto.AuthorName, ct) ?? new Author(dto.AuthorName);
        book.GetType().GetProperty("AuthorId")!.SetValue(book, author.Id);

        book.UpdateDetails(dto.Title, dto.TotalChapters, dto.Genre, dto.Isbn, dto.Format, dto.Publisher, dto.Language, dto.Pages, dto.Year);
        db.Update(book);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] AppDbContext db, CancellationToken ct)
    {
        var book = await db.Books.FindAsync(new object[] { id }, ct);
        if (book is null) return NotFound();
        db.Remove(book);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}

