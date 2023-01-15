namespace Spotty.App;

public interface ISpottyState
{
    SpottyQuiz[] GetQuizzes();
}

public class SpottyState : ISpottyState
{
    private SpottyQuiz[] Quizzes { get; }

    public SpottyState(SpottyQuiz[] quizzes)
    {
        Quizzes = quizzes;
    }

    public SpottyQuiz[] GetQuizzes()
    {
        return Quizzes;
    }
}

public class SpottyQuiz
{
    public string Title { get; set; } = string.Empty;

    public List<SpottyQuestion> Questions { get; set; } = new List<SpottyQuestion>();
}

public class SpottyQuestion
{
    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public List<SpottyTrack> Tracks { get; set; } = new List<SpottyTrack>();
}

public class SpottyTrack
{
    public string SpotifyUrl { get; set; } = string.Empty;

    public int Offset { get; set; }

    public int Duration { get; set; }
}
