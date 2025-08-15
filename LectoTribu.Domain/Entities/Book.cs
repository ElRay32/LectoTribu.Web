using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; private set; }
    public Guid AuthorId { get; private set; }
    public int TotalChapters { get; private set; }
    public string? Isbn { get; private set; }

    public Book(string title, Guid authorId, int totalChapters, string? isbn = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título requerido", nameof(title));
        if (totalChapters <= 0) throw new ArgumentOutOfRangeException(nameof(totalChapters));
        Title = title.Trim();
        AuthorId = authorId;
        TotalChapters = totalChapters;
        Isbn = isbn;
    }
}
