namespace Spotty.WebApp.App;

public class SpotifyException : Exception
{
    public SpotifyException()
    {
    }

    public SpotifyException(string message) : base(message)
    {
    }

    public SpotifyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
