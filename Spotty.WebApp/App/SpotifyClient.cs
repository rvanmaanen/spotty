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

        return devicesResponse.GetCurrentDeviceId();
    }

    private static async Task GetContentAndThrowException(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<dynamic>(content);

        throw new SpotifyException($"Spotify API returned status code {response.StatusCode} with content {error}");
    }
}
