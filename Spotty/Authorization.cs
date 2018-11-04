using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        private string RedirectUrl { get; }

        public Authorization(string clientId, string clientSecret, Uri redirectUrl)
            : this(clientId, clientSecret, redirectUrl.AbsolutePath)
        {
        }

        public Authorization(string clientId, string clientSecret, string redirectUrl)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUrl = WebUtility.UrlDecode(redirectUrl);
        }

        public Uri GetRedirectUriForCode(string scope)
        {
            var encodedRedirectUrl = WebUtility.UrlEncode(RedirectUrl);

            return new Uri($"https://accounts.spotify.com/authorize/?client_id={ClientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}");
        }

        public async Task<string> GetTokenAsync(string code)
        {
            var httpClient = HttpClientFactory.Create();

            var uri = new Uri("https://accounts.spotify.com/api/token");
            var content = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUrl }
            };

            var tokenResponse = await httpClient.PostAsync(uri, new FormUrlEncodedContent(content)).ConfigureAwait(false);

            if(!tokenResponse.IsSuccessStatusCode)
            {
                throw new ApplicationException("Failed to get token: " + tokenResponse.ReasonPhrase);
            }

            return await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
