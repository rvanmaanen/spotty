using System.Text.Json;

namespace Spotty.WebApp.App.Quizzes;

public class Quizzes(string contentRootFilePath) : IQuizzes
{
    private readonly string _contentRootFilePath = contentRootFilePath;

    public Quiz[] GetQuizzes()
    {
        var quizzes = new List<Quiz>();
        var quizFilesPath = Path.Combine(_contentRootFilePath, "wwwroot/quizzes");

        var quizFiles = Directory.GetFiles(quizFilesPath);

        if (quizFiles is null || quizFiles.Length == 0)
        {
            throw new ArgumentException("No quizzes found in wwwroot");
        }

        foreach (var quizFile in quizFiles)
        {
            var quiz = JsonSerializer.Deserialize<Quiz>(File.ReadAllText(quizFile)) ?? throw new ArgumentException($"Quiz {quizFile} could not be deserialized");

            quizzes.Add(quiz);
        }

        return [.. quizzes];
    }
}
