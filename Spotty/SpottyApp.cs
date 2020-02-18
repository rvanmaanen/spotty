﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace Spotty
{
    public interface ISpottyApp
    {
        Uri GetUrlForLoginCode();

        Task Login(string code);

        Task Pause();

        Task Play(string track, int position);

        bool IsLoggedIn();
    }

    public class SpottyApp : ISpottyApp
    {
        private string ClientId { get; }
        private string ClientSecret { get; }
        private Uri RedirectUrl { get; }
        private ISpotifyHttpClient SpotifyHttpClient { get; }

        private string AccessToken { get; set; }
        private string RefreshToken { get; set; }

        public SpottyApp(string clientId, string clientSecret, Uri redirectUrl, ISpotifyHttpClient spotifyHttpClient)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUrl = redirectUrl;
            SpotifyHttpClient = spotifyHttpClient;
        }

        public Uri GetUrlForLoginCode()
        {
            var scope = WebUtility.UrlEncode("user-modify-playback-state user-read-playback-state");
            var encodedRedirectUrl = WebUtility.UrlEncode(RedirectUrl.AbsoluteUri);

            return new Uri($"https://accounts.spotify.com/authorize/?client_id={ClientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}");
        }

        public async Task Login(string code)
        {
            var (accessToken, refreshToken) = await SpotifyHttpClient.GetTokenAsync(ClientId, ClientSecret, RedirectUrl, code).ConfigureAwait(false);

            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public async Task Pause()
        {
            SpotifyHttpClient.SetAuthorization(AccessToken);

            await SpotifyHttpClient.Pause().ConfigureAwait(false);
        }

        public async Task Play(string track, int position = 0)
        {
            SpotifyHttpClient.SetAuthorization(AccessToken);

            await SpotifyHttpClient.Play(track, position).ConfigureAwait(false);
        }

        public bool IsLoggedIn()
        {
            return AccessToken != null;
        }
    }
}
