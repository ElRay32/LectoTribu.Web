using System;
using System.Collections.Generic;
using System.Linq;                 // ← necesario para FirstOrDefault
using System.Threading.Tasks;
using LectoTribu.Web.Services;
using LectoTribu.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ReadModel : PageModel
{
    private readonly IClubsApi _clubsApi;
    private readonly IBooksApi _booksApi;
    private readonly IUsersApi _usersApi;

    public ReadModel(IClubsApi clubsApi, IBooksApi booksApi, IUsersApi usersApi)
    {
        _clubsApi = clubsApi;
        _booksApi = booksApi;
        _usersApi = usersApi;
    }

    // Parámetros de la sala de lectura (query string)
    [BindProperty(SupportsGet = true)] public Guid ClubId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid BookId { get; set; }
    [BindProperty(SupportsGet = true)] public int Chapter { get; set; } = 1;

    // Cabecera actual
    public string HeaderTitle => $"Sala de lectura – Cap. {Chapter}";

    // NUEVO: nombres a mostrar y propiedades que pide la vista
    public string ClubName { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string ChapterTitle => string.IsNullOrWhiteSpace(BookTitle) ? $"Cap. {Chapter}" : $"{BookTitle} – Cap. {Chapter}";
    public string ChapterText { get; set; } = string.Empty; // aquí puedes poner el texto real si lo tuvieras

    // Listas para los combos del formulario
    public List<ClubItem> Clubs { get; set; } = new();
    public List<BookItem> Books { get; set; } = new();

    // Comentarios existentes
    public List<CommentVm> Comments { get; set; } = new();

    // Modelo del formulario de nuevo comentario
    [BindProperty] public CommentInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Cargar combos
        Clubs = await _clubsApi.GetAllAsync();
        Books = await _booksApi.GetAllAsync();

        // Resolver selección actual
        var selectedClubId = ClubId != Guid.Empty ? ClubId : (Clubs.Count > 0 ? Clubs[0].Id : Guid.Empty);
        var selectedBookId = BookId != Guid.Empty ? BookId : (Books.Count > 0 ? Books[0].Id : Guid.Empty);

        // Nombres para la vista
        ClubName = Clubs.FirstOrDefault(c => c.Id == selectedClubId)?.Name ?? string.Empty;
        BookTitle = Books.FirstOrDefault(b => b.Id == selectedBookId)?.Title ?? string.Empty;

        // (Opcional) texto del capítulo si tuvieras contenido; por ahora vacío
        ChapterText = string.Empty;

        // Cargar comentarios
        if (selectedClubId != Guid.Empty && selectedBookId != Guid.Empty && Chapter > 0)
        {
            Comments = await _clubsApi.GetCommentsAsync(selectedClubId, selectedBookId, Chapter);
        }

        // Inicializar formulario
        Input = new CommentInput
        {
            SelectedClubId = selectedClubId == Guid.Empty ? "" : selectedClubId.ToString(),
            SelectedBookId = selectedBookId == Guid.Empty ? "" : selectedBookId.ToString(),
            Chapter = Chapter
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validaciones de entrada
        if (!Guid.TryParse(Input.SelectedClubId, out var club))
            ModelState.AddModelError(string.Empty, "Seleccione un club válido.");
        if (!Guid.TryParse(Input.SelectedBookId, out var book))
            ModelState.AddModelError(string.Empty, "Seleccione un libro válido.");
        if (Input.Chapter <= 0)
            ModelState.AddModelError(string.Empty, "El capítulo debe ser mayor que 0.");
        if (string.IsNullOrWhiteSpace(Input.UserName))
            ModelState.AddModelError(string.Empty, "Ingrese su nombre.");

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        // Obtener/crear usuario por nombre
        var userId = await _usersApi.GetOrCreateByNameAsync(Input.UserName.Trim());

        // Publicar comentario
        await _clubsApi.CommentAsync(club, book, Input.Chapter, userId, Input.Content);

        // Volver a la sala de lectura con la selección actual
        return RedirectToPage("/Read/Index", new { clubId = club, bookId = book, chapter = Input.Chapter });
    }

    public class CommentInput
    {
        public string SelectedClubId { get; set; } = string.Empty;
        public string SelectedBookId { get; set; } = string.Empty;
        public int Chapter { get; set; } = 1;
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
