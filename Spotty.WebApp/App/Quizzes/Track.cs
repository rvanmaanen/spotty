namespace Spotty.WebApp.App.Quizzes;

public class Track
{
    public string SpotifyUrl { get; set; } = "";

    public int Offset { get; set; }

    public int Duration { get; set; }

    public int Group { get; set; }
}
