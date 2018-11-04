using Xunit;
using FluentAssertions;
using System;
using Moq;
using System.Net;

namespace Spotty.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public void ShouldCreateProperRedirectUriForCode()
        {
            const string clientId = "1234";
            const string clientSecret = "4567";
            const string scope = "app-remote-control";
            var redirectUrl = new Uri("http://localhost:56081/index/authorizationcallback");
            var encodedRedirectUrl = WebUtility.UrlEncode(redirectUrl.AbsoluteUri);
            var spotifyHttpClient = Mock.Of<ISpotifyHttpClient>();
            var authorization = (IAuthorization) new Authorization(clientId, clientSecret, redirectUrl, spotifyHttpClient);

            var authorizationUri = authorization.GetRedirectUriForCode(scope);

            authorizationUri.Should().NotBeNull();
            authorizationUri.Should().BeEquivalentTo(new Uri($"https://accounts.spotify.com/authorize/?client_id={clientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}"));
        }
    }
}
