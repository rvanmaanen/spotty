using Newtonsoft.Json;
using Spotty.App;
using Spotty.Client;

var builder = CreateWebApplicationBuilder(args);

ConfigurePipeline(builder);

static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Services.AddRazorPages();

    builder.Services
        .AddHttpClient("SpotifyHttpClient", client =>
        {
            client.DefaultRequestHeaders.Remove("Accept");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddTypedClient<ISpotifyClient>(httpClient => new SpotifyClient(httpClient));

    builder.Services.AddSingleton<ISpottyApp>(serviceProvider => new SpottyApp(
        configuration.GetValue<string>("Spotty:ClientId"),
        configuration.GetValue<string>("Spotty:ClientSecret"),
        new Uri(configuration.GetValue<string>("Spotty:SpotifyAuthenticationCallbackUrl")),
        serviceProvider.GetRequiredService<ISpotifyClient>()
    ));

    builder.Services.AddSingleton<ISpottyState>(sp => new SpottyState(GetQuizzes(builder.Environment)));

    return builder;
}

static SpottyQuiz[] GetQuizzes(IWebHostEnvironment environment)
{
    var quizzes = new List<SpottyQuiz>();
    var quizFiles = environment.ContentRootFileProvider.GetDirectoryContents("wwwroot/quizzes");

    foreach (var quizFile in quizFiles)
    {
        using Stream stream = new FileStream(quizFile.PhysicalPath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        quizzes.Add(JsonConvert.DeserializeObject<SpottyQuiz>(reader.ReadToEnd()));
    }

    return quizzes.ToArray();
}

static void ConfigurePipeline(WebApplicationBuilder builder)
{
    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
}
