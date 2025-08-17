using System.Net.Http.Json;
using System.Linq;

namespace LectoTribu.Web.Services;

public interface IUsersApi
{
    Task<Guid> CreateAsync(string firstName, string lastName, string? email = null);

 
    Task<Guid?> FindByNameAsync(string name);

   
    Task<Guid> GetOrCreateByNameAsync(string fullName);
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

    public async Task<Guid?> FindByNameAsync(string name)
    {
        var list = await _http.GetFromJsonAsync<List<UserItem>>("/api/users") ?? new();
        var match = list.FirstOrDefault(u => string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase));
        return match?.Id;
    }

    public async Task<Guid> GetOrCreateByNameAsync(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Name is required", nameof(fullName));

        var existing = await FindByNameAsync(fullName.Trim());
        if (existing.HasValue) return existing.Value;

        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var first = parts.Length > 0 ? parts[0] : fullName.Trim();
        var last = parts.Length > 1 ? parts[1] : "";
        return await CreateAsync(first, last, null);
    }

    private sealed record UserItem(Guid Id, string Name);
}
