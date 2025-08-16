using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;
using LectoTribu.Domain.ValueObjects;

namespace LectoTribu.Domain.Entities;

public class Review : Post
{
    public Guid BookId { get; private set; }
    public Rating Rating { get; private set; } = null!; // EF la pobla

    private Review() : base() { } // <--- para EF

    public Review(Guid userId, Guid bookId, Rating rating, string content)
        : base(userId, content)
    {
        BookId = bookId;
        Rating = rating;
    }

    public override string Kind => "review";
}
