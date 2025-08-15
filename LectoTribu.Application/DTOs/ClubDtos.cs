using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectoTribu.Application.DTOs;

public record CreateClubDto(string Name, Guid OwnerId, string? Description);
public record AddBookDto(Guid ClubId, Guid BookId);
public record AddMemberByEmailDto(Guid ClubId, string Email);
public record ScheduleChapterDto(Guid ClubId, Guid BookId, int Chapter, DateOnly Date);
public record SchedulePlanDto(Guid ClubId, Guid BookId, DateOnly StartDate, int ChaptersPerWeek, int TotalChapters);
public record CommentDto(Guid ClubId, Guid BookId, int Chapter, Guid UserId, string Content);
