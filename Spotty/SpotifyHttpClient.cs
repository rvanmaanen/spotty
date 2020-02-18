using Spotty.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Spotty
{
    public interface ISpotifyHttpClient
    {
        Task<(string accessToken, string refreshToken)> GetTokenAsync(string clientId, string clientSecret, Uri redirectUrl, string code);

        void SetAuthorization(string token);

        Task Pause();

        Task Play(string track, int position);
    }

    public class SpotifyHttpClient : ISpotifyHttpClient
    {
        private HttpClient HttpClient { get; }

        public SpotifyHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
            HttpClient.DefaultRequestHeaders.Remove("Accept");
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public void SetAuthorization(string token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<(string accessToken, string refreshToken)> GetTokenAsync(string clientId, string clientSecret, Uri redirectUrl, string code)
        {
            var uri = new Uri("https://accounts.spotify.com/api/token");
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
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyAuthenticationError>(content);
                throw new LoginFailedException($"Spotify API returned status code {error.Error} with message {error.Error_Description}");
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);
            return (result.access_token, result.refresh_token);
        }

        public async Task Pause()
        {
            var deviceId = await GetDeviceId().ConfigureAwait(false);
            var uri = new Uri($"https://api.spotify.com/v1/me/player/pause?device_id={deviceId}");
            var body = new Dictionary<string, string>();
            var response = await HttpClient.PutAsync(uri, new FormUrlEncodedContent(body)).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyRegularError>(content);
                throw new PlayerFailedException($"Spotify API returned status code {error.Error.Status} with message {error.Error.Message}");
            }
        }

        public async Task Play(string track, int position = 0)
        {
            var deviceId = await GetDeviceId().ConfigureAwait(false);
            var uri = new Uri($"https://api.spotify.com/v1/me/player/play?device_id={deviceId}");
            var body = new StringContent("{ \"uris\": [\"" + track + "\"], \"position_ms\": " + position + "}");
            var response = await HttpClient.PutAsync(uri, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyRegularError>(content);
                throw new PlayerFailedException($"Spotify API returned status code {error.Error.Status} with message {error.Error.Message}");
            }
        }

        private async Task<string> GetDeviceId()
        {
            var uri = new Uri($"https://api.spotify.com/v1/me/player/devices");
            var response = await HttpClient.GetAsync(uri).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyRegularError>(content);
                throw new PlayerFailedException($"Spotify API returned status code {error.Error.Status} with message {error.Error.Message}");
            }

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SpotifyDevices>(content);
            if (result.Devices.Length == 0)
            {
                throw new PlayerFailedException("Failed to find any devices to use");
            }

            var activeDevice = result.Devices.SingleOrDefault(device => device.Is_Active);
            if (activeDevice != null)
            {
                return activeDevice.Id;
            }

            return result.Devices.First().Id;
        }
    }

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1716 // Identifiers should not match keywords
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public class SpotifyRegularError
    {
        public Error Error;
    }

    public class Error
    {
        public int Status;
        public string Message;
    }

    public class SpotifyAuthenticationError
    {
        public string Error;
        public string Error_Description;
    }

    public class SpotifyDevices
    {
        public Device[] Devices;
    }

    public class Device
    {
        public string Id;
        public bool Is_Active;
        public bool Is_Private_Session;
        public bool Is_Restricted;
        public string Name;
        public string Type;
        public int Volume_Percent;
    }
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1716 // Identifiers should not match keywords
#pragma warning restore CA1707 // Identifiers should not contain underscores
}


