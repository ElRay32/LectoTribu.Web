using System.Net.Http.Json;
using LectoTribu.Web.ViewModels;
using LectoTribu.Web.Services;

namespace LectoTribu.Web.Services;

public class ClubsApi : IClubsApi
{
    private readonly HttpClient _http;
    public ClubsApi(HttpClient http) => _http = http;

    // --- Listado / CRUD ---
    public async Task<List<ClubItem>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<ClubItem>>("/api/clubs") ?? new();

    public async Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description)
    {
        var res = await _http.PostAsJsonAsync("/api/clubs", new { name, ownerId, description });
        res.EnsureSuccessStatusCode();
        var dto = await res.Content.ReadFromJsonAsync<CreatedClubDto>();
        return dto!.Id;
    }

    public async Task UpdateAsync(Guid id, string name, string? description)
    {
        var res = await _http.PutAsJsonAsync($"/api/clubs/{id}", new { name, description });
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var res = await _http.DeleteAsync($"/api/clubs/{id}");
        res.EnsureSuccessStatusCode();
    }

    // --- Acciones ---
    public async Task ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date)
    {
        var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/schedule/one",
            new { clubId = Guid.Empty, bookId, chapter, date });
        res.EnsureSuccessStatusCode();
    }

    public async Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content)
    {
        var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/comment",
            new { clubId = Guid.Empty, bookId, chapter, userId, content });
        res.EnsureSuccessStatusCode();
    }
    public async Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter)
    {
        var url = $"/api/clubs/{clubId}/comments?bookId={bookId}&chapter={chapter}";
        return await _http.GetFromJsonAsync<List<CommentVm>>(url) ?? new();
    }

    private sealed record CreatedClubDto(Guid Id, string Name);
}





