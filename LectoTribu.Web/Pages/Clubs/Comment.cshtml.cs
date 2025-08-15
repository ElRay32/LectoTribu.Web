using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CommentModel : PageModel
{
    private readonly IClubsApi _api;
    public CommentModel(IClubsApi api) => _api = api;

    [BindProperty] public CommentInput Input { get; set; } = new();
    public bool Sent { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Guid.TryParse(Input.ClubId, out var club) ||
            !Guid.TryParse(Input.BookId, out var book) ||
            !Guid.TryParse(Input.UserId, out var user))
        {
            ModelState.AddModelError(string.Empty, "Alguno de los GUIDs es inválido.");
            return Page();
        }

        await _api.CommentAsync(club, book, Input.Chapter, user, Input.Content);
        Sent = true;
        return Page();
    }

    public class CommentInput
    {
        public string ClubId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}