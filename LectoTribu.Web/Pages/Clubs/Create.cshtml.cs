using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateModel : PageModel
{
    private readonly IClubsApi _api;
    public CreateModel(IClubsApi api) => _api = api;

    [BindProperty] public CreateClubInput Input { get; set; } = new();
    public Guid CreatedClubId { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!Guid.TryParse(Input.OwnerId, out var ownerGuid))
        {
            ModelState.AddModelError(string.Empty, "OwnerId no es un GUID válido.");
            return Page();
        }
        CreatedClubId = await _api.CreateClubAsync(Input.Name, ownerGuid, Input.Description);
        return Page();
    }

    public class CreateClubInput
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string OwnerId { get; set; } = string.Empty;
    }
}
