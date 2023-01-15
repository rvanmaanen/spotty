namespace Spotty.App;

public interface ISpottyApp
{
    Uri GetUrlForLoginCode();

    Task Login(string code);

    Task Pause();

    Task Play(string track, int position);

    bool IsLoggedIn();
}
