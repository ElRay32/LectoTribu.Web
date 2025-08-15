using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Application.DTOs;
using LectoTribu.Domain.Entities;

namespace LectoTribu.Application.Interfaces;

public interface IClubService
{
    Task<Club> CreateClubAsync(CreateClubDto dto, CancellationToken ct = default);
    Task AddBookAsync(AddBookDto dto, CancellationToken ct = default);
    Task AddMemberAsync(Guid clubId, Guid userId, CancellationToken ct = default);                 // overload 1
    Task AddMemberAsync(AddMemberByEmailDto dto, CancellationToken ct = default);                  // overload 2
    Task ScheduleAsync(ScheduleChapterDto dto, CancellationToken ct = default);                    // overload 1
    Task ScheduleAsync(SchedulePlanDto dto, CancellationToken ct = default);                       // overload 2
    Task CommentAsync(CommentDto dto, CancellationToken ct = default);
}
