using System.Net.Http.Json;

namespace LectoTribu.Web.Services;

public interface IUsersApi
{
    Task<Guid> CreateAsync(string firstName, string lastName, string? email = null);
}

public class UsersApi : IUsersApi
{
    private readonly HttpClient _http;
    public UsersApi(HttpClient http) => _http = http;

    public async Task<Guid> CreateAsync(string firstName, string lastName, string? email = null)
    {
        var res = await _http.PostAsJsonAsync("/api/users", new { firstName, lastName, email });
        res.EnsureSuccessStatusCode();
        var dto = await res.Content.ReadFromJsonAsync<UserItem>();
        return dto!.Id;
    }

    private sealed record UserItem(Guid Id, string Name);
}