using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ClubEditModel : PageModel
{
    private readonly IClubsApi _api;
    public ClubEditModel(IClubsApi api) => _api = api;

    [BindProperty] public ClubEditInput Input { get; set; } = new();

    public async Task OnGetAsync(Guid id)
    {
        var list = await _api.GetAllAsync();
        var item = list.First(x => x.Id == id);
        Input = new ClubEditInput { Id = item.Id, Name = item.Name, Description = item.Description };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _api.UpdateAsync(Input.Id, Input.Name, Input.Description);
        return RedirectToPage("/Clubs/Index");
    }

    public class ClubEditInput { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string? Description { get; set; } }
}