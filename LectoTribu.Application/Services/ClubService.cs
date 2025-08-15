using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Application.DTOs;
using LectoTribu.Application.Interfaces;
using LectoTribu.Domain.Abstractions;
using LectoTribu.Domain.Entities;

namespace LectoTribu.Application.Services;

public class ClubService : IClubService
{
    private readonly IRepository<Club> _clubs;
    private readonly IRepository<User> _users;
    private readonly IRepository<Book> _books;
    private readonly IRepository<ChapterComment> _comments;
    private readonly IUnitOfWork _uow;
    private readonly IMessageBus _bus;

    public ClubService(IRepository<Club> clubs, IRepository<User> users, IRepository<Book> books,
        IRepository<ChapterComment> comments, IUnitOfWork uow, IMessageBus bus)
    {
        _clubs = clubs; _users = users; _books = books; _comments = comments; _uow = uow; _bus = bus;
    }

    public async Task<Club> CreateClubAsync(CreateClubDto dto, CancellationToken ct)
    {
        var club = new Club(dto.Name, dto.OwnerId, dto.Description);
        await _clubs.AddAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
        await _bus.PublishAsync(new { Type = "ClubCreated", club.Id, club.Name });
        return club;
    }

    public async Task AddBookAsync(AddBookDto dto, CancellationToken ct)
    {
        var club = await _clubs.GetByIdAsync(dto.ClubId, ct) ?? throw new InvalidOperationException("Club no existe");
        var book = await _books.GetByIdAsync(dto.BookId, ct) ?? throw new InvalidOperationException("Libro no existe");
        club.AddBook(book.Id);
        await _clubs.UpdateAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task AddMemberAsync(Guid clubId, Guid userId, CancellationToken ct)
    {
        var club = await _clubs.GetByIdAsync(clubId, ct) ?? throw new InvalidOperationException("Club no existe");
        _ = await _users.GetByIdAsync(userId, ct) ?? throw new InvalidOperationException("Usuario no existe");
        club.AddMember(userId);
        await _clubs.UpdateAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task AddMemberAsync(AddMemberByEmailDto dto, CancellationToken ct)
    {
        var club = await _clubs.GetByIdAsync(dto.ClubId, ct) ?? throw new InvalidOperationException("Club no existe");
        Guid ResolveUserId(string email) => throw new NotImplementedException("Resolver usuario por email en repositorio");
        club.AddMember(dto.Email, ResolveUserId);
        await _clubs.UpdateAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ScheduleAsync(ScheduleChapterDto dto, CancellationToken ct)
    {
        var club = await _clubs.GetByIdAsync(dto.ClubId, ct) ?? throw new InvalidOperationException("Club no existe");
        club.ScheduleChapter(dto.BookId, dto.Chapter, dto.Date);
        await _clubs.UpdateAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ScheduleAsync(SchedulePlanDto dto, CancellationToken ct)
    {
        var club = await _clubs.GetByIdAsync(dto.ClubId, ct) ?? throw new InvalidOperationException("Club no existe");
        club.ScheduleChapter(dto.BookId, dto.StartDate, dto.ChaptersPerWeek, dto.TotalChapters);
        await _clubs.UpdateAsync(club, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task CommentAsync(CommentDto dto, CancellationToken ct)
    {
        var comment = new ChapterComment(dto.UserId, dto.ClubId, dto.BookId, dto.Chapter, dto.Content);
        await _comments.AddAsync(comment, ct);
        await _uow.SaveChangesAsync(ct);
        await _bus.PublishAsync(new { Type = "ChapterCommentCreated", comment.Id, dto.ClubId, dto.BookId, dto.Chapter });
    }
}
