namespace Spotty.Client
{
    public interface ISpotifyClient
    {
        Task<(string accessToken, string refreshToken)> GetTokensAsync(string clientId, string clientSecret, Uri redirectUrl, string code);

        Task Pause(string accessToken);

        Task Play(string accessToken, string track, int position);
    }
}


