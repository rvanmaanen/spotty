using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Spotty.WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            services.AddSingleton<ISpottyState, SpottyState>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc();
        }
    }
}
