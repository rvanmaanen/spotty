using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Spotty.App;

public interface ISpottyState
{
    SpottyQuiz[] GetQuizzes();
}

public class SpottyState : ISpottyState
{
    private readonly string _contentRootFilePath;

    public SpottyState(string contentRootFilePath)
    {
        _contentRootFilePath = contentRootFilePath;
    }

    public SpottyQuiz[] GetQuizzes()
    {
        var quizzes = new List<SpottyQuiz>();
        var quizFilesPath = Path.Combine(_contentRootFilePath, "wwwroot/quizzes");

        var quizFiles = Directory.GetFiles(quizFilesPath);

        if (quizFiles is null || !quizFiles.Any())
        {
            throw new ArgumentException("No quizzes found in wwwroot");
        }

        foreach (var quizFile in quizFiles)
        {
            var quiz = JsonConvert.DeserializeObject<SpottyQuiz>(File.ReadAllText(quizFile));
            if (quiz is null)
            {
                throw new ArgumentException($"Quiz {quizFile} could not be deserialized");
            }

            quizzes.Add(quiz);
        }

        return quizzes.ToArray();
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
    private string _spotifyUrl = string.Empty;

    public string SpotifyUrl
    {
        get => _spotifyUrl;
        set => _spotifyUrl = Regex.Replace(value, "\\?.*", "");
    }

    public int Offset { get; set; }

    public int Duration { get; set; }
}
