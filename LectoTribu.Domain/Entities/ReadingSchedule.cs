using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class ReadingSchedule : BaseEntity
{
    public Guid ClubId { get; private set; }
    public Guid BookId { get; private set; }
    public int ChapterNumber { get; private set; }
    public DateOnly PlannedDate { get; private set; }

    public ReadingSchedule(Guid clubId, Guid bookId, int chapterNumber, DateOnly plannedDate)
    {
        if (chapterNumber <= 0) throw new ArgumentOutOfRangeException(nameof(chapterNumber));
        ClubId = clubId;
        BookId = bookId;
        ChapterNumber = chapterNumber;
        PlannedDate = plannedDate;
    }
}