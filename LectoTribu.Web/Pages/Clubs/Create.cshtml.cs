using LectoTribu.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateClubModel : PageModel
{
    private readonly IUsersApi _users;
    private readonly IClubsApi _clubs;
    public CreateClubModel(IUsersApi users, IClubsApi clubs) { _users = users; _clubs = clubs; }

    [BindProperty] public CreateClubInput Input { get; set; } = new();
    public Guid CreatedClubId { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var ownerId = await _users.CreateAsync(Input.OwnerFirstName, Input.OwnerLastName);
        CreatedClubId = await _clubs.CreateClubAsync(Input.ClubName, ownerId, Input.Description);
        return Page();
    }

    public class CreateClubInput
    {
        public string ClubName { get; set; } = string.Empty;
        public string OwnerFirstName { get; set; } = string.Empty;
        public string OwnerLastName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
