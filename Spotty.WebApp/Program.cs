using Newtonsoft.Json;
using Spotty.App;
using Spotty.Client;

var builder = CreateWebApplicationBuilder(args);

ConfigurePipeline(builder);

static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration!;

    builder.Services.AddRazorPages();

    builder.Services
        .AddHttpClient("SpotifyHttpClient", client =>
        {
            client.DefaultRequestHeaders.Remove("Accept");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddTypedClient<ISpotifyClient>(httpClient => new SpotifyClient(httpClient));

    builder.Services.AddSingleton<ISpottyApp>(serviceProvider => new SpottyApp(
        configuration.GetValue<string>("Spotty:ClientId") ?? throw new ArgumentException("Spotty:ClientId is not configured"),
        configuration.GetValue<string>("Spotty:ClientSecret") ?? throw new ArgumentException("Spotty:ClientSecret is not configured"),
        new Uri(configuration.GetValue<string>("Spotty:SpotifyAuthenticationCallbackUrl") ?? throw new ArgumentException("Spotty:SpotifyAuthenticationCallbackUrl is not configured")),
        serviceProvider.GetRequiredService<ISpotifyClient>()
    ));

    builder.Services.AddSingleton<ISpottyState>(sp => new SpottyState(GetQuizzes(builder.Environment)));

    return builder;
}

static SpottyQuiz[] GetQuizzes(IWebHostEnvironment environment)
{
    var quizzes = new List<SpottyQuiz>();
    var quizFiles = environment.ContentRootFileProvider.GetDirectoryContents("wwwroot/quizzes");

    if (quizFiles is null || !quizFiles.Any())
    {
        throw new ArgumentException("No quizzes found in wwwroot");
    }

    foreach (var quizFile in quizFiles)
    {
        using Stream stream = new FileStream(quizFile.PhysicalPath!, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        var quiz = JsonConvert.DeserializeObject<SpottyQuiz>(reader.ReadToEnd());
        if (quiz is null)
        {
            throw new ArgumentException($"Quiz {quizFile.PhysicalPath} could not be deserialized");
        }

        quizzes.Add(quiz);
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
