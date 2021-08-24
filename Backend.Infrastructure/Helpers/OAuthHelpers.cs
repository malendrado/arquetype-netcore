using System;
using RestSharp;

namespace Backend.Infrastructure.Helpers
{
    public static class OAuthHelpers
    {

        public static IRestRequest CreateAccessTokenRequest(string code, Uri redirectUri, string consumerKey, string consumerSecret)
        {
            var request = GetOauthRequest();
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", consumerKey);
            request.AddParameter("client_secret", consumerSecret);
            request.AddParameter("redirect_uri", redirectUri);
            return request;
        }


        public static IRestRequest RefreshAccessTokenRequest(string consumerKey, string consumerSecret, string refreshToken)
        {
            var request = GetOauthRequest();
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", consumerKey);
            request.AddParameter("client_secret", consumerSecret);
            request.AddParameter("refresh_token", refreshToken);
            return request;
        }


        public static IRestRequest GetAccessTokenRequest(string consumerKey, string consumerSecret)
        {
            var request = GetOauthRequest();
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", consumerKey);
            request.AddParameter("client_secret", consumerSecret);
            //request.AddParameter("refresh_token", refreshToken);
            return request;
        }


        public static IRestRequest GetAccessTokenV1Request(string authorization)
        {
            var request = GetOauthRequest();
            request.AddHeader("authorization", authorization);
            request.AddParameter("grant_type", "client_credentials");
            return request;
        }

        private static IRestRequest GetOauthRequest()
        {
            return new RestRequest(Method.POST)
            {
                Resource = "/token",
                RequestFormat = DataFormat.Json
            };
        }
    }
}