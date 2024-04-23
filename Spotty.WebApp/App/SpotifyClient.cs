using System.Text.Json;
using Spotty.WebApp.App.Models;

namespace Spotty.WebApp.App;

public class SpotifyClient(HttpClient httpClient) : ISpotifyClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task Pause()
    {
        var deviceId = await GetCurrentDeviceId();
        var uri = new Uri(_httpClient.BaseAddress + $"/pause?device_id={deviceId}");
        var body = new Dictionary<string, string>();
        var response = await _httpClient.PutAsync(uri, new FormUrlEncodedContent(body));

        if (!response.IsSuccessStatusCode)
        {
            await GetContentAndThrowException(response);
        }
    }

    public async Task Play(string track, int position)
    {
        var deviceId = await GetCurrentDeviceId();
        var uri = new Uri(_httpClient.BaseAddress + $"/play?device_id={deviceId}");
        var body = new StringContent("{ \"uris\": [\"" + track + "\"], \"position_ms\": " + position + "}");
        var response = await _httpClient.PutAsync(uri, body);

        if (!response.IsSuccessStatusCode)
        {
            await GetContentAndThrowException(response);
        }
    }

    private async Task<string> GetCurrentDeviceId()
    {
        var uri = new Uri(_httpClient.BaseAddress + "/devices");
        var response = await _httpClient.GetAsync(uri);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            await GetContentAndThrowException(response);
        }

        var devicesResponse = JsonSerializer.Deserialize<DevicesResponse>(content) ?? throw new SpotifyException("Failed to deserialize /devices response");

        var devices = devicesResponse.devices;
        if (devices.Length == 0)
        {
            throw new SpotifyException("No devices found, open Spotify somewhere");
        }

        var activeDevice = devices.SingleOrDefault(device => device.is_active);
        if (activeDevice != null)
        {
            return activeDevice.id;
        }

        if (devices.Length == 1)
        {
            var body = new StringContent("{ \"device_ids\": [\"" + devices[0].id + "\"] }");
            response = await _httpClient.PutAsync(_httpClient.BaseAddress, body);

            if (!response.IsSuccessStatusCode)
            {
                await GetContentAndThrowException(response);
            }

            return devices[0].id;
        }

        throw new SpotifyException("Multiple devices found, activate Spotify somewhere by playing a song");
    }

    private static async Task GetContentAndThrowException(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<dynamic>(content);

        throw new SpotifyException($"Spotify API returned status code {response.StatusCode} with content {error}");
    }
}
