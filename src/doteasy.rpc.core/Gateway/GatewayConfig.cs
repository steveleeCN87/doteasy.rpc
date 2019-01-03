using System;
using Microsoft.Extensions.Configuration;

namespace DotEasy.Rpc.Core.Gateway
{
    public static class GatewayConfig
    {
        public static IConfigurationRoot Configuration => null;

        public static string _authorizationServiceKey;
        public static string AuthorizationServiceKey => Configuration["AuthorizationServiceKey"] ?? _authorizationServiceKey;

        public static string _authorizationRoutePath;
        public static string AuthorizationRoutePath => Configuration["AuthorizationRoutePath"] ?? _authorizationRoutePath;

        private static TimeSpan _accessTokenExpireTimeSpan = TimeSpan.FromMinutes(30);

        public static TimeSpan AccessTokenExpireTimeSpan
        {
            get
            {
                int tokenExpireTime;
                if (Configuration["AccessTokenExpireTimeSpan"] != null && int.TryParse(Configuration["AccessTokenExpireTimeSpan"], out tokenExpireTime))
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

        private static string _cacheMode = "MemoryCache";
        public static string CacheMode => Configuration["CacheMode"] ?? _cacheMode;
    }
}