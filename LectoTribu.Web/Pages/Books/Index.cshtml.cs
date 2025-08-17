using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BooksIndexModel : PageModel
{
    private readonly IBooksApi _api;
    public BooksIndexModel(IBooksApi api) => _api = api;
    public List<BookItem> Items { get; set; } = new();

    public async Task OnGetAsync() => Items = await _api.GetAllAsync();

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _api.DeleteAsync(id);
        return RedirectToPage();
    }
}