namespace Spotty.WebApp.App.Spotty.Models;

public class Round
{
    public string Title { get; set; } = string.Empty;

    public List<Track> Tracks { get; set; } = [];
}
