using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LectoTribu.Web.Services
{
   
    public record CommentVm(Guid Id, Guid UserId, string Content, DateTime CreatedAtUtc);

    public interface IClubsApi
    {
        Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description);
        Task ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date);
        Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content);

        
        Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter);
    }
}
