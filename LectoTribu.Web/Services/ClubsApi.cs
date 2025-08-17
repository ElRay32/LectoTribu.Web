using System.Net.Http.Json;
using LectoTribu.Web.ViewModels;

namespace LectoTribu.Web.Services
{
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
            var created = await res.Content.ReadFromJsonAsync<CreatedClubDto>();
            return created?.Id ?? Guid.Empty;
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
        // Maneja 400/404 sin tirar excepción en la Razor Page
        public async Task<(bool Ok, string? Error)> ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date)
        {
            var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/schedule/one",
                new { bookId, chapter, date });

            if (res.IsSuccessStatusCode) return (true, null);

            // Intentamos leer el mensaje; si viene vacío, devolvemos el StatusCode
            var msg = await res.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(msg) ? res.StatusCode.ToString() : msg);
        }

        public async Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content)
        {
            // El controller establece ClubId desde la ruta; no es necesario mandarlo en el body
            var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/comment",
                new { bookId, chapter, userId, content });
            res.EnsureSuccessStatusCode();
        }

        public async Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter)
        {
            var url = $"/api/clubs/{clubId}/comments?bookId={bookId}&chapter={chapter}";
            return await _http.GetFromJsonAsync<List<CommentVm>>(url) ?? new();
        }

        // DTO interno para leer el Id devuelto por POST /api/clubs
        private sealed record CreatedClubDto(Guid Id, string Name);
    }
}






