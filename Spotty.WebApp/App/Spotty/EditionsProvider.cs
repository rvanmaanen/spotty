using System.Text.Json;
using Spotty.WebApp.App.Spotty.Models;

namespace Spotty.WebApp.App.Spotty;

public class EditionsProvider : IEditionsProvider
{
    private readonly Dictionary<string, Round[]> _editions;

    public EditionsProvider(string contentRootFilePath)
    {
        _editions = [];

        var editionsFilesPath = Path.Combine(contentRootFilePath, "wwwroot", "editions");
        var editionsDirectories = Directory.GetDirectories(editionsFilesPath).OrderDescending();

        foreach (var editionDirectory in editionsDirectories)
        {
            var rounds = new List<Round>();
            var roundsFiles = Directory.GetFiles(editionDirectory);

            foreach (var roundsFile in roundsFiles)
            {
                var round = JsonSerializer.Deserialize<Round>(File.ReadAllText(roundsFile)) ?? throw new ArgumentException($"Round {roundsFile} could not be deserialized");

                rounds.Add(round);
            }

            var edition = editionDirectory.Substring(editionDirectory.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            _editions.Add(edition, [.. rounds]);
        }
    }

    public Dictionary<string, Round[]> GetEditions()
    {
        return _editions;
    }

    public string GetLatestEditionName()
    {
        return _editions.First().Key;
    }
}
