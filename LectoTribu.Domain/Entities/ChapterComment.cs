using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class ChapterComment : Post
{
    public Guid ClubId { get; private set; }
    public Guid BookId { get; private set; }
    public int ChapterNumber { get; private set; }

    public ChapterComment(Guid userId, Guid clubId, Guid bookId, int chapterNumber, string content)
        : base(userId, content)
    {
        if (chapterNumber <= 0) throw new ArgumentOutOfRangeException(nameof(chapterNumber));
        ClubId = clubId;
        BookId = bookId;
        ChapterNumber = chapterNumber;
    }

    public override string Kind => "comment";
}
