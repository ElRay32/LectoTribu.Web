using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class Club : BaseEntity
{
    private readonly List<Membership> _members = new();
    private readonly List<ReadingSchedule> _schedules = new();
    private readonly List<Guid> _books = new();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<Membership> Members => _members;
    public IReadOnlyCollection<ReadingSchedule> Schedules => _schedules;
    public IReadOnlyCollection<Guid> Books => _books;

    public Club(string name, Guid ownerId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido", nameof(name));
        Name = name.Trim();
        OwnerId = ownerId;
        Description = description;
        _members.Add(new Membership(ownerId, Id, ClubRole.Owner));
    }

    public void AddBook(Guid bookId)
    {
        if (_books.Contains(bookId)) return;
        _books.Add(bookId);
        Touch();
    }

    // Sobrecarga 1: agregar miembro por Guid
    public void AddMember(Guid userId, ClubRole role = ClubRole.Member)
    {
        if (_members.Any(m => m.UserId == userId)) return;
        _members.Add(new Membership(userId, Id, role));
        Touch();
    }

    // Sobrecarga 2: agregar miembro por email (resolveUserId lo provee Application)
    public void AddMember(string userEmail, Func<string, Guid> resolveUserId, ClubRole role = ClubRole.Member)
    {
        var userId = resolveUserId(userEmail);
        AddMember(userId, role);
    }

    // Sobrecarga 1: programar capítulo puntual
    public void ScheduleChapter(Guid bookId, int chapterNumber, DateOnly date)
    {
        if (chapterNumber <= 0) throw new ArgumentOutOfRangeException(nameof(chapterNumber));
        _schedules.Add(new ReadingSchedule(Id, bookId, chapterNumber, date));
        Touch();
    }

    // Sobrecarga 2: plan semanal
    public void ScheduleChapter(Guid bookId, DateOnly startDate, int chaptersPerWeek, int totalChapters)
    {
        if (chaptersPerWeek <= 0) throw new ArgumentOutOfRangeException(nameof(chaptersPerWeek));
        if (totalChapters <= 0) throw new ArgumentOutOfRangeException(nameof(totalChapters));
        var chapter = 1;
        var date = startDate;
        while (chapter <= totalChapters)
        {
            for (int i = 0; i < chaptersPerWeek && chapter <= totalChapters; i++)
            {
                _schedules.Add(new ReadingSchedule(Id, bookId, chapter++, date));
            }
            date = date.AddDays(7);
        }
        Touch();
    }
    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido", nameof(name));
        Name = name.Trim();
        Description = description;
        Touch();
    }
}

public enum ClubRole { Owner, Moderator, Member }
