using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Spotty.WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("SpotifyHttpClient", client =>
            {
                client.DefaultRequestHeaders.Remove("Accept");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddTypedClient<ISpotifyClient>(httpClient => new SpotifyClient(httpClient));

            services.AddSingleton<ISpottyApp>(serviceProvider => new SpottyApp(
                Configuration.GetValue<string>("Spotty:ClientId"),
                Configuration.GetValue<string>("Spotty:ClientSecret"),
                new Uri(Configuration.GetValue<string>("Spotty:SpotifyAuthenticationCallbackUrl")),
                serviceProvider.GetService<ISpotifyClient>()
            ));

            services.AddSingleton<ISpottyState>(sp => new SpottyState(GetQuizzes()));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        private SpottyQuiz[] GetQuizzes()
        {
            var quizzes = new List<SpottyQuiz>();
            var quizFiles = HostEnvironment.ContentRootFileProvider.GetDirectoryContents("wwwroot/quizzes");
            foreach (var quizFile in quizFiles)
            {
                using Stream stream = new FileStream(quizFile.PhysicalPath, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream);
                quizzes.Add(JsonConvert.DeserializeObject<SpottyQuiz>(reader.ReadToEnd()));
            }

            return quizzes.ToArray();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
