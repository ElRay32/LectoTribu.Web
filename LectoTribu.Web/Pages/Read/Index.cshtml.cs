using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LectoTribu.Web.Services;

public class ReadModel : PageModel
{
    private readonly IClubsApi _api;
    public ReadModel(IClubsApi api) => _api = api;

    // Parámetros de la vista
    [BindProperty(SupportsGet = true)] public Guid ClubId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid BookId { get; set; }
    [BindProperty(SupportsGet = true)] public int Chapter { get; set; } = 1;

    public string HeaderTitle => "Lectura del capítulo";
    public string ChapterTitle => $"Capítulo {Chapter}";
    public string ChapterText => "Texto del capítulo (placeholder). Aquí se mostraría el contenido del libro o un resumen autorizado.";

    public List<CommentVm> Comments { get; set; } = new();

    // Formulario de comentario
    [BindProperty] public CommentInput Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Comments = await _api.GetCommentsAsync(ClubId, BookId, Chapter);
        Input = new CommentInput { ClubId = ClubId, BookId = BookId, Chapter = Chapter };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Guid.TryParse(Input.UserId, out var user))
        {
            ModelState.AddModelError(string.Empty, "UserId inválido");
            await OnGetAsync();
            return Page();
        }
        await _api.CommentAsync(Input.ClubId, Input.BookId, Input.Chapter, user, Input.Content);
        return RedirectToPage(new { ClubId = Input.ClubId, BookId = Input.BookId, Chapter = Input.Chapter });
    }

    public class CommentInput
    {
        public Guid ClubId { get; set; }
        public Guid BookId { get; set; }
        public int Chapter { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}