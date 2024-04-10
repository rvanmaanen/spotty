using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Spotty.WebApp.App;

namespace Spotty.WebApp;

public class AccessTokenMessageHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //IdentityConstants.ApplicationScheme
        //SpotifyAuthenticationDefaults.AuthenticationScheme

        var httpContext = _httpContextAccessor.HttpContext ?? throw new SpotifyException("No HTTP Context available");
        var accessToken = await httpContext.GetTokenAsync("access_token") ?? throw new SpotifyException("No Access Token available");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
