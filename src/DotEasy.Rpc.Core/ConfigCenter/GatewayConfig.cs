using System;
using Microsoft.Extensions.Configuration;

namespace DotEasy.Rpc.Core.ConfigCenter
{
    public static class GatewayConfig
    {
        private static IConfigurationRoot Configuration => null;

#pragma warning disable 649
        private static string _authorizationServiceKey;
#pragma warning restore 649
        public static string AuthorizationServiceKey => Configuration["AuthorizationServiceKey"] ?? _authorizationServiceKey;

#pragma warning disable 649
        private static string _authorizationRoutePath;
#pragma warning restore 649
        public static string AuthorizationRoutePath => Configuration["AuthorizationRoutePath"] ?? _authorizationRoutePath;

        private static TimeSpan _accessTokenExpireTimeSpan = TimeSpan.FromMinutes(30);

        public static TimeSpan AccessTokenExpireTimeSpan
        {
            get
            {
                if (Configuration["AccessTokenExpireTimeSpan"] != null && int.TryParse(Configuration["AccessTokenExpireTimeSpan"], out var tokenExpireTime))
                {
                    _accessTokenExpireTimeSpan = TimeSpan.FromMinutes(tokenExpireTime);
                }

                return _accessTokenExpireTimeSpan;
            }
        }

        private static string _tokenEndpointPath = "oauth2/token";

        public static string TokenEndpointPath
        {
            get => Configuration["TokenEndpointPath"] ?? _tokenEndpointPath;
            internal set => _tokenEndpointPath = value;
        }

        private static string _cacheMode = "DefaultMemoryCache";
        public static string CacheMode => "DefaultRedisCache";
    }
}