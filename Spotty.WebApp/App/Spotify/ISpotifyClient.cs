namespace Spotty.WebApp.App.Spotify;

public interface ISpotifyClient
{
    Task Pause();

    Task Play(string track, int position);
}
