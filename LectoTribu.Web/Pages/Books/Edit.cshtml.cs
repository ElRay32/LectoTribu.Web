using LectoTribu.Web.Services;
using LectoTribu.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BookEditModel : PageModel
{
    private readonly IBooksApi _api;
    public BookEditModel(IBooksApi api) => _api = api;

    [BindProperty] public BookEditInput Input { get; set; } = new();

    public async Task OnGetAsync(Guid id)
    {
        var list = await _api.GetAllAsync();
        var b = list.First(x => x.Id == id);
        // Nota: aquí solo traemos Título; completa campos manualmente o crea un GET detallado si quieres
        Input = new BookEditInput { Id = b.Id, Title = b.Title, AuthorName = "", TotalChapters = 1, Genre = "General", Format = BookFormat.Ebook };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _api.UpdateAsync(Input.Id, Input.Title, Input.AuthorName, Input.TotalChapters,
                               Input.Genre, Input.Format, Input.Publisher, Input.Language, Input.Pages, Input.Year);
        return RedirectToPage("/Books/Index");
    }

    public class BookEditInput
    {
        public Guid Id { get; set; }
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
