using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using LectoTribu.Web.Services;  
using Microsoft.AspNetCore.Mvc.RazorPages;
using static LectoTribu.Web.Services.ClubsApi;

public class ClubsIndexModel : PageModel
{
    private readonly IClubsApi _api;
    public ClubsIndexModel(IClubsApi api) => _api = api;
    public List<ClubItem> Items { get; set; } = new();

    public async Task OnGetAsync() => Items = await _api.GetAllAsync();

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _api.DeleteAsync(id);
        return RedirectToPage();
    }
}