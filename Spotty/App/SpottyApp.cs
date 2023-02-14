using System.Net;
using Spotty.Client;

namespace Spotty.App;

public class SpottyApp : ISpottyApp, IDisposable
{
    private CancellationTokenSource _pauseCancellationTokenSource = new();

    private bool _disposedValue;

    private string ClientId { get; }
    private string ClientSecret { get; }
    private Uri RedirectUrl { get; }
    private ISpotifyClient SpotifyClient { get; }

    private string AccessToken { get; set; } = string.Empty;
    private string RefreshToken { get; set; } = string.Empty;

    public SpottyApp(string clientId, string clientSecret, Uri redirectUrl, ISpotifyClient spotifyClient)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        RedirectUrl = redirectUrl;
        SpotifyClient = spotifyClient;
    }

    public Uri GetUrlForLoginCode()
    {
        var scope = WebUtility.UrlEncode("user-modify-playback-state user-read-playback-state");
        var encodedRedirectUrl = WebUtility.UrlEncode(RedirectUrl.AbsoluteUri);

        return new Uri($"https://accounts.spotify.com/authorize/?client_id={ClientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}");
    }

    public async Task Login(string code)
    {
        (var accessToken, var refreshToken) = await SpotifyClient.GetTokensAsync(ClientId, ClientSecret, RedirectUrl, code).ConfigureAwait(false);

        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public async Task Pause()
    {
        await SpotifyClient.Pause(AccessToken).ConfigureAwait(false);
    }

    public async Task Play(string track, int position)
    {
        await SpotifyClient.Play(AccessToken, track, position).ConfigureAwait(false);
    }

    public async Task PlayAndPause(string track, int position, int duration)
    {
        _pauseCancellationTokenSource.Cancel();
        _pauseCancellationTokenSource = new CancellationTokenSource();

        var pauseCancellationToken = _pauseCancellationTokenSource.Token;

        await SpotifyClient.Play(AccessToken, track, position).ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            await Task.Delay(duration, pauseCancellationToken).ConfigureAwait(false);

            if (!pauseCancellationToken.IsCancellationRequested)
            {
                await SpotifyClient.Pause(AccessToken).ConfigureAwait(false);
            }
        }, pauseCancellationToken);
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(AccessToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _pauseCancellationTokenSource.Cancel();
                _pauseCancellationTokenSource.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
