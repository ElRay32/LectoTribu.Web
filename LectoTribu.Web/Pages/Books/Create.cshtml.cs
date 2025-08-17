using LectoTribu.Web.Services;
using LectoTribu.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BookCreateModel : PageModel
{
    private readonly IBooksApi _api;
    public BookCreateModel(IBooksApi api) => _api = api;

    [BindProperty] public BookInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        await _api.CreateAsync(Input.Title, Input.AuthorName, Input.TotalChapters,
                               Input.Genre, Input.Format, Input.Publisher, Input.Language, Input.Pages, Input.Year);
        return RedirectToPage("/Books/Index");
    }

    public class BookInput
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int TotalChapters { get; set; } = 1;

        public string Genre { get; set; } = "General";
        public BookFormat Format { get; set; } = BookFormat.Ebook;
        public string? Publisher { get; set; }
        public string? Language { get; set; }
        public int? Pages { get; set; }
        public int? Year { get; set; }
    }
}

