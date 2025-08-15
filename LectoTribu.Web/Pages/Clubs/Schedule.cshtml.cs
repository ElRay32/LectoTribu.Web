using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ScheduleModel : PageModel
{
    private readonly IClubsApi _api;
    public ScheduleModel(IClubsApi api) => _api = api;

    [BindProperty] public ScheduleInput Input { get; set; } = new();
    public bool Saved { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Guid.TryParse(Input.ClubId, out var club) || !Guid.TryParse(Input.BookId, out var book))
        {
            ModelState.AddModelError(string.Empty, "ClubId o BookId inválidos.");
            return Page();
        }
        await _api.ScheduleOneAsync(club, book, Input.Chapter, DateOnly.FromDateTime(Input.Date));
        Saved = true;
        return Page();
    }

    public class ScheduleInput
    {
        public string ClubId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public int Chapter { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
    }
}