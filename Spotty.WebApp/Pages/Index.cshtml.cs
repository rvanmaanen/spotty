using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spotty.WebApp.App.Spotify;
using Spotty.WebApp.App.Spotty;
using Spotty.WebApp.App.Spotty.Models;

namespace Spotty.WebApp.Pages;

[Authorize]
public class IndexModel(ISpotifyClient spotifyClient, IEditionsProvider editionsProvider) : PageModel
{
    public string Edition
    {
        get
        {
            if (HttpContext.Request.Query.ContainsKey("edition"))
            {
                return HttpContext.Request.Query["edition"].Single()!;
            }

            return editionsProvider.GetLatestEditionName();
        }
    }

    public Round[] Rounds
    {
        get
        {
            if(Edition != "")
            {
                if (editionsProvider.GetEditions().TryGetValue(Edition, out var edition))
                {
                    return edition;
                }
            }

            return []; 
        }
    }

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
