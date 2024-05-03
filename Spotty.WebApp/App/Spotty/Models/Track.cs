namespace Spotty.WebApp.App.Spotty.Models;

public class Track
{
    public string SpotifyUrl { get; set; } = "";

    public int Offset { get; set; }

    public int Duration { get; set; }

    public int Group { get; set; }
}
