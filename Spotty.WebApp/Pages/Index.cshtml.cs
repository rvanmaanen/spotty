using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading;
using System.Threading.Tasks;

namespace Spotty.WebApp.Pages
{
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
            await SpottyApp.Login(code).ConfigureAwait(false);

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

            await PlaySong(track, offset, duration).ConfigureAwait(false);
        }

        private async Task PlaySong(string track, int offset, int duration)
        {
            await SpottyApp.Play(track, offset).ConfigureAwait(false);

            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                Thread.Sleep(duration);

                SpottyApp.Pause();
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
