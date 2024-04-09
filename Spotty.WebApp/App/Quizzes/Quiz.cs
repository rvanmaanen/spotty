namespace Spotty.WebApp.App.Quizzes;

public class Quiz
{
    public string Title { get; set; } = string.Empty;

    public List<Track> Tracks { get; set; } = [];
}
