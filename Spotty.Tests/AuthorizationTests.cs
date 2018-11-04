using Xunit;
using FluentAssertions;
using System;
using System.Net;

namespace Spotty.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public void ShouldCreateProperRedirectUriForCode_WithoutEncodedRedirectUrl()
        {
            const string clientId = "1234";
            const string clientSecret = "4567";
            const string scope = "app-remote-control";
            var redirectUrl = "http://localhost:56081/index/authorizationcallback";
            var authorization = (IAuthorization) new Authorization(clientId, clientSecret, redirectUrl);

            var authorizationUri = authorization.GetRedirectUriForCode(scope);

            var encodedRedirectUrl = WebUtility.UrlEncode(redirectUrl);
            authorizationUri.Should().NotBeNull();
            authorizationUri.Should().BeEquivalentTo(new Uri($"https://accounts.spotify.com/authorize/?client_id={clientId}&response_type=code&redirect_uri={encodedRedirectUrl}&scope={scope}"));
        }

        [Fact]
        public void ShouldCreateProperRedirectUriForCode_WithEncodedRedirectUrl()
        {
            const string clientId = "1234";
            const string clientSecret = "4567";
            const string scope = "app-remote-control";
            var redirectUrl = WebUtility.UrlEncode("http://localhost:56081/index/authorizationcallback");
            var authorization = (IAuthorization) new Authorization(clientId, clientSecret, redirectUrl);

            var authorizationUri = authorization.GetRedirectUriForCode(scope);

            authorizationUri.Should().NotBeNull();
            authorizationUri.Should().BeEquivalentTo(new Uri($"https://accounts.spotify.com/authorize/?client_id={clientId}&response_type=code&redirect_uri={redirectUrl}&scope={scope}"));
        }
    }
}
