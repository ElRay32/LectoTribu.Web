using System.Net.Http.Json;
using LectoTribu.Web.ViewModels;

namespace LectoTribu.Web.Services;

public record BookItem(Guid Id, string Title);

public interface IBooksApi
{
    Task<List<BookItem>> GetAllAsync();
    Task<Guid> CreateAsync(string title, string authorName, int totalChapters,
                           string genre, BookFormat format,
                           string? publisher, string? language, int? pages, int? year);
    Task UpdateAsync(Guid id, string title, string authorName, int totalChapters,
                     string genre, BookFormat format,
                     string? publisher, string? language, int? pages, int? year);
    Task DeleteAsync(Guid id);
}

public class BooksApi : IBooksApi
{
    private readonly HttpClient _http;
    public BooksApi(HttpClient http) => _http = http;

    public async Task<List<BookItem>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<BookItem>>("/api/books") ?? new();

    public async Task<Guid> CreateAsync(string title, string authorName, int totalChapters,
                                        string genre, BookFormat format,
                                        string? publisher, string? language, int? pages, int? year)
    {
        var res = await _http.PostAsJsonAsync("/api/books",
                     new { title, authorName, totalChapters, genre, format, publisher, language, pages, year });
        res.EnsureSuccessStatusCode();
        var dto = await res.Content.ReadFromJsonAsync<BookItem>();
        return dto!.Id;
    }

    public async Task UpdateAsync(Guid id, string title, string authorName, int totalChapters,
                                  string genre, BookFormat format,
                                  string? publisher, string? language, int? pages, int? year)
    {
        var res = await _http.PutAsJsonAsync($"/api/books/{id}",
                     new { title, authorName, totalChapters, genre, format, publisher, language, pages, year });
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var res = await _http.DeleteAsync($"/api/books/{id}");
        res.EnsureSuccessStatusCode();
    }
}

