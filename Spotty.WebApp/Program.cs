using AspNet.Security.OAuth.Spotify;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Spotty.WebApp;
using Spotty.WebApp.App;
using Spotty.WebApp.App.Quizzes;
using Spotty.WebApp.Data;

var builder = CreateWebApplicationBuilder(args);

ConfigurePipeline(builder);

static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<SpottyDbContext>(options => options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<SpottyDbContext>();
    builder.Services.AddRazorPages();
    builder.Services.AddAuthentication(SpotifyAuthenticationDefaults.AuthenticationScheme)
                    .AddSpotify(options =>
                    {
                        options.ClientId = configuration.GetValue<string>("Spotty:ClientId")!;
                        options.ClientSecret = configuration.GetValue<string>("Spotty:ClientSecret")!;
                        options.CallbackPath = "/signin-oidc";
                        options.SaveTokens = true;
                        options.Scope.Add("user-modify-playback-state");
                        options.Scope.Add("user-read-playback-state");
                    });

    builder.Services.AddTransient<AccessTokenMessageHandler>();

    builder.Services
        .AddHttpClient("SpotifyApi", client =>
        {
            client.BaseAddress = new Uri("https://api.spotify.com/v1/me/player");
            client.DefaultRequestHeaders.Remove("Accept");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AccessTokenMessageHandler>()
        .AddTypedClient<ISpotifyClient>(httpClient => new SpotifyClient(httpClient))
        .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100)));

    builder.Services.AddTransient<IQuizzes>(sp => new Quizzes(builder.Environment.ContentRootPath));

    return builder;
}

static void ConfigurePipeline(WebApplicationBuilder builder)
{
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();
    app.Run();
}
