using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spotty.App;

namespace Spotty.WebApp.Pages;

public class IndexModel : PageModel
{
    public bool IsLoggedIn { get; }
    public ISpottyState SpottyState { get; }

    private ISpottyApp SpottyApp { get; }

    public IndexModel(ISpottyApp spottyApp, ISpottyState spottyState)
    {
        SpottyApp = spottyApp;
        SpottyState = spottyState;

        IsLoggedIn = SpottyApp.IsLoggedIn();
    }

    public async Task<IActionResult> OnGetAuthorizationCallbackAsync(string code)
    {
        await SpottyApp.Login(code);

        return Redirect("/");
    }

    public IActionResult OnPostLogin()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return Redirect(SpottyApp.GetUrlForLoginCode().AbsoluteUri);
    }

    public async Task OnPostPlay(string track, int offset, int duration)
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        await SpottyApp.PlayAndPause(track, offset, duration);
    }
}
