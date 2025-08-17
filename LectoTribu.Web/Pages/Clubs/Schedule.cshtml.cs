using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LectoTribu.Web.Services;
using static LectoTribu.Web.Services.ClubsApi;

public class ScheduleModel : PageModel
{
    private readonly IClubsApi _clubsApi; private readonly IBooksApi _booksApi;
    public ScheduleModel(IClubsApi clubsApi, IBooksApi booksApi) { _clubsApi = clubsApi; _booksApi = booksApi; }

    [BindProperty] public ScheduleInput Input { get; set; } = new();
    public bool Saved { get; set; }
    public List<ClubItem> Clubs { get; set; } = new();
    public List<BookItem> Books { get; set; } = new();

    public async Task OnGetAsync()
    {
        Clubs = await _clubsApi.GetAllAsync();
        Books = await _booksApi.GetAllAsync();
        if (Clubs.Count > 0) Input.ClubId = Clubs[0].Id.ToString();
        if (Books.Count > 0) Input.BookId = Books[0].Id.ToString();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Guid.TryParse(Input.ClubId, out var club) || !Guid.TryParse(Input.BookId, out var book))
        { ModelState.AddModelError(string.Empty, "Selección inválida"); await OnGetAsync(); return Page(); }
        await _clubsApi.ScheduleOneAsync(club, book, Input.Chapter, DateOnly.FromDateTime(Input.Date));
        Saved = true;
        await OnGetAsync();
        return Page();
    }

    public class ScheduleInput
    {
        public string ClubId { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public int Chapter { get; set; } = 1;
        public DateTime Date { get; set; } = DateTime.Today;
    }

}