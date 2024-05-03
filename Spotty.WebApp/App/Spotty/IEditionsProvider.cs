using Spotty.WebApp.App.Spotty.Models;

namespace Spotty.WebApp.App.Spotty;

public interface IEditionsProvider
{
    Dictionary<string, Round[]> GetEditions();

    string GetLatestEditionName();
}
