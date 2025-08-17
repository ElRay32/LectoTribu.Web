using LectoTribu.Web.ViewModels;
using LectoTribu.Web.Services;

namespace LectoTribu.Web.Services;


public record ClubItem(Guid Id, string Name, string? Description);

public interface IClubsApi
{
    // CRUD básico
    Task<List<ClubItem>> GetAllAsync();
    Task UpdateAsync(Guid id, string name, string? description);
    Task DeleteAsync(Guid id);

    // Acciones
    Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description);
    Task ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date);
    Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content);
    Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter);
}



