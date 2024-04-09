using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spotty.WebApp.App;
using Spotty.WebApp.App.Quizzes;

namespace Spotty.WebApp.Pages;

[Authorize]
public class IndexModel(ISpotifyClient spotifyClient, IQuizzes quizzes) : PageModel
{
    public Quiz[] Quizzes { get; } = quizzes.GetQuizzes();

    private ISpotifyClient SpotifyClient { get; } = spotifyClient;

    public async Task OnGetPlay(string track, int offset)
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        await SpotifyClient.Play(track, offset);
    }

    public async Task OnGetPause()
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        await SpotifyClient.Pause();
    }
}
