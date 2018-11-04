using System;
using System.Net;
using System.Threading.Tasks;

namespace Spotty
{
    public interface IAuthorization
    {
        Uri GetRedirectUriForCode(string scope);

        Task<string> GetTokenAsync(string code);
    }

    public class Authorization : IAuthorization
    {
        private string ClientId { get; }
        private string ClientSecret { get; }
        private Uri RedirectUrl { get; }
        private ISpotifyHttpClient SpotifyHttpClient { get; }

        public Authorization(string clientId, string clientSecret, Uri redirectUrl, ISpotifyHttpClient spotifyHttpClient)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUrl = redirectUrl;
            SpotifyHttpClient = spotifyHttpClient;
        }

        public Uri GetRedirectUriForCode(string scope)
        {
            var encodedRedirectUrl = WebUtility.UrlEncode(RedirectUrl.AbsoluteUri);

            return new Uri($"https://accounts.spotify.com/authorize/?client_id={ClientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}");
        }

        public async Task<string> GetTokenAsync(string code)
        {
            return await SpotifyHttpClient.GetTokenAsync(ClientId, ClientSecret, RedirectUrl, code).ConfigureAwait(false);
        }
    }
}
