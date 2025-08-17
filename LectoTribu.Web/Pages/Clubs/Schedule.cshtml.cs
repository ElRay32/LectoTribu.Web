using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ScheduleModel : PageModel
{
    private readonly IClubsApi _clubsApi;
    private readonly IBooksApi _booksApi;

    public ScheduleModel(IClubsApi clubsApi, IBooksApi booksApi)
    {
        _clubsApi = clubsApi;
        _booksApi = booksApi;
    }

    [BindProperty] public ScheduleInput Input { get; set; } = new();
    public bool Saved { get; set; }
    public List<ClubItem> Clubs { get; set; } = new();
    public List<BookItem> Books { get; set; } = new();

    public async Task OnGetAsync()
    {
        Clubs = await _clubsApi.GetAllAsync();
        Books = await _booksApi.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validaciones básicas
        if (!Guid.TryParse(Input.ClubId, out var club))
            ModelState.AddModelError(string.Empty, "Seleccione un club válido.");
        if (!Guid.TryParse(Input.BookId, out var book))
            ModelState.AddModelError(string.Empty, "Seleccione un libro válido.");
        if (Input.Chapter <= 0)
            ModelState.AddModelError(string.Empty, "El capítulo debe ser mayor que 0.");

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var (ok, error) = await _clubsApi.ScheduleOneAsync(
            club, book, Input.Chapter, DateOnly.FromDateTime(Input.Date));

        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "No se pudo programar el capítulo.");
            await OnGetAsync();
            return Page();
        }

        //  Redirige a la Sala de lectura con los parámetros correctos
        return RedirectToPage("/Read/Index", new { clubId = club, bookId = book, chapter = Input.Chapter });
    }

    public class ScheduleInput
    {
        public string ClubId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public int Chapter { get; set; } = 1;
        public DateTime Date { get; set; } = DateTime.Today;
    }
}

