using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spotty
{
    public interface ISpotifyHttpClient
    {
        Task<string> GetTokenAsync(string clientId, string clientSecret, Uri redirectUrl, string code);
    }

    public class SpotifyHttpClient : ISpotifyHttpClient
    {
        private HttpClient HttpClient { get; }

        public SpotifyHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<string> GetTokenAsync(string clientId, string clientSecret, Uri redirectUrl, string code)
        {
            var uri = new Uri("https://accounts.spotify.com/api/token");
            var content = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUrl.AbsoluteUri }
            };

            var tokenResponse = await HttpClient.PostAsync(uri, new FormUrlEncodedContent(content)).ConfigureAwait(false);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new ApplicationException("Failed to get token: " + tokenResponse.ReasonPhrase);
            }

            return await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
