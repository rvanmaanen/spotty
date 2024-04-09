namespace Spotty.WebApp.App;

public interface ISpotifyClient
{
    Task Pause();

    Task Play(string track, int position);
}
