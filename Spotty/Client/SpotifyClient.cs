using System.Net.Http.Headers;
using Newtonsoft.Json;
using Spotty.Client.Models;

namespace Spotty.Client
{
    public class SpotifyClient : ISpotifyClient
    {
        private const string SpotifyBaseUrl = "https://api.spotify.com";
        private const string SpotifyPlayerUrl = SpotifyBaseUrl + "/v1/me/player";
        private const string SpotifyTokenUrl = "https://accounts.spotify.com/api/token";

        private HttpClient HttpClient { get; }

        public SpotifyClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<(string accessToken, string refreshToken)> GetTokensAsync(string clientId, string clientSecret, Uri redirectUrl, string code)
        {
            var uri = new Uri(SpotifyTokenUrl);
            var body = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUrl.AbsoluteUri }
            };
            var response = await HttpClient.PostAsync(uri, new FormUrlEncodedContent(body)).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                ThrowException(response, content);
            }

            var result = JsonConvert.DeserializeObject<dynamic>(content);

            return (result.access_token, result.refresh_token);
        }

        public async Task Pause(string accessToken)
        {
            SetAuthorizationHeader(accessToken);

            var deviceId = await GetDeviceId().ConfigureAwait(false);
            var uri = new Uri($"{SpotifyPlayerUrl}/pause?device_id={deviceId}");
            var body = new Dictionary<string, string>();
            var response = await HttpClient.PutAsync(uri, new FormUrlEncodedContent(body)).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await GetContentAndThrowException(response).ConfigureAwait(false);
            }
        }

        public async Task Play(string accessToken, string track, int position = 0)
        {
            SetAuthorizationHeader(accessToken);

            var deviceId = await GetDeviceId().ConfigureAwait(false);
            var uri = new Uri($"{SpotifyPlayerUrl}/play?device_id={deviceId}");
            var body = new StringContent("{ \"uris\": [\"" + track + "\"], \"position_ms\": " + position + "}");
            var response = await HttpClient.PutAsync(uri, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await GetContentAndThrowException(response).ConfigureAwait(false);
            }
        }

        private async Task<string> GetDeviceId()
        {
            var uri = new Uri($"{SpotifyPlayerUrl}/devices");
            var response = await HttpClient.GetAsync(uri).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                ThrowException(response, content);
            }

            var result = JsonConvert.DeserializeObject<SpotifyDevices>(content);
            if (result.Devices.Length == 0)
            {
                throw new SpotifyException("Failed to find any devices to use");
            }

            var activeDevice = result.Devices.SingleOrDefault(device => device.Is_Active);
            if (activeDevice != null)
            {
                return activeDevice.Id;
            }

            throw new SpotifyException("No active device found");
        }

        private static async Task GetContentAndThrowException(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            ThrowException(response, content);
        }

        private static void ThrowException(HttpResponseMessage response, string content)
        {
            var error = JsonConvert.DeserializeObject(content);

            throw new SpotifyException($"Spotify API returned status code {response.StatusCode} with content {error}");
        }

        private void SetAuthorizationHeader(string accessToken)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}


