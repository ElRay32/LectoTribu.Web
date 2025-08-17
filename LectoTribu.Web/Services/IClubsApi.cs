using LectoTribu.Web.ViewModels;

namespace LectoTribu.Web.Services
{
    public record ClubItem(Guid Id, string Name, string? Description);

    public interface IClubsApi
    {
        // CRUD básico
        Task<List<ClubItem>> GetAllAsync();
        Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description);
        Task UpdateAsync(Guid id, string name, string? description);
        Task DeleteAsync(Guid id);

        // Acciones
        // ⬇️ Devolvemos (Ok, Error) para no romper la UI con excepción cuando la API responde 400/404
        Task<(bool Ok, string? Error)> ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date);

        Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content);
        Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter);
    }
}




