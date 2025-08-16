using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LectoTribu.Web.Services
{
    public class ClubsApi : IClubsApi
    {
        private readonly HttpClient _http;
        public ClubsApi(HttpClient http) => _http = http;

        public async Task<Guid> CreateClubAsync(string name, Guid ownerId, string? description)
        {
            var dto = new { name, ownerId, description };
            var res = await _http.PostAsJsonAsync("/api/clubs", dto);
            res.EnsureSuccessStatusCode();
            var created = await res.Content.ReadFromJsonAsync<ClubCreatedDto>();
            return created!.Id;
        }

        public async Task ScheduleOneAsync(Guid clubId, Guid bookId, int chapter, DateOnly date)
        {
            var dto = new { clubId = Guid.Empty, bookId, chapter, date };
            var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/schedule/one", dto);
            res.EnsureSuccessStatusCode();
        }

        public async Task CommentAsync(Guid clubId, Guid bookId, int chapter, Guid userId, string content)
        {
            var dto = new { clubId = Guid.Empty, bookId, chapter, userId, content };
            var res = await _http.PostAsJsonAsync($"/api/clubs/{clubId}/comment", dto);
            res.EnsureSuccessStatusCode();
        }

        
        public async Task<List<CommentVm>> GetCommentsAsync(Guid clubId, Guid bookId, int chapter)
        {
            var url = $"/api/clubs/{clubId}/comments?bookId={bookId}&chapter={chapter}";
            var list = await _http.GetFromJsonAsync<List<CommentVm>>(url);
            return list ?? new();
        }

        private sealed record ClubCreatedDto(Guid Id, string Name);
    }
}


