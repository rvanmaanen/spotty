namespace Spotty.App;

public interface ISpottyApp
{
    Uri GetUrlForLoginCode();

    Task Login(string code);

    Task Pause();

    Task Play(string track, int position);

    Task PlayAndPause(string track, int position, int duration);

    bool IsLoggedIn();
}
