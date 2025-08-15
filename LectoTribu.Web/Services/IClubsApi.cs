namespace LectoTribu.Web.Services;

public interface IClubsApi
{
    Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description);
    Task ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date);
    Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content);
}
