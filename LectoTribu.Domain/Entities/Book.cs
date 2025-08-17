using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public enum BookFormat { Unknown = 0, Ebook, Paperback, Hardcover, Audiobook }

public class Book : BaseEntity
{
    public string Title { get; private set; }
    public Guid AuthorId { get; private set; }
    public int TotalChapters { get; private set; }
    public string? Isbn { get; private set; }

    // NUEVOS
    public string Genre { get; private set; } = "General";
    public BookFormat Format { get; private set; } = BookFormat.Ebook;
    public string? Publisher { get; private set; }
    public string? Language { get; private set; }
    public int? Pages { get; private set; }
    public int? Year { get; private set; }

    public Book(string title, Guid authorId, int totalChapters,
                string genre = "General", BookFormat format = BookFormat.Ebook,
                string? publisher = null, string? language = null, int? pages = null, int? year = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título requerido", nameof(title));
        if (totalChapters <= 0) throw new ArgumentOutOfRangeException(nameof(totalChapters));
        Title = title.Trim();
        AuthorId = authorId;
        TotalChapters = totalChapters;

        Genre = string.IsNullOrWhiteSpace(genre) ? "General" : genre.Trim();
        Format = format;
        Publisher = publisher;
        Language = language;
        Pages = pages;
        Year = year;
    }

    public void UpdateDetails(string title, int totalChapters, string isbn,
                              string genre, BookFormat format,
                              string? publisher, string? language, int? pages, int? year)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título requerido", nameof(title));
        if (totalChapters <= 0) throw new ArgumentOutOfRangeException(nameof(totalChapters));
        Title = title.Trim();
        TotalChapters = totalChapters;
        Isbn = isbn;
        Genre = string.IsNullOrWhiteSpace(genre) ? "General" : genre.Trim();
        Format = format;
        Publisher = publisher;
        Language = language;
        Pages = pages;
        Year = year;
        Touch();
    }
}


