using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotEasy.Rpc.Cache;
using DotEasy.Rpc.Cache.Caching;
using DotEasy.Rpc.Proxy;
using DotEasy.Rpc.Routing;
using Newtonsoft.Json;

namespace DotEasy.Rpc.Gateway.OAuth.Impl
{
    public class AuthorizationServerProvider : IAuthorizationServerProvider
    {
        private readonly IServiceProxyProvider _serviceProxyProvider;
        private readonly IServiceRouteProvider _serviceRouteProvider;
        private readonly ICacheProvider _cacheProvider;

        public AuthorizationServerProvider(IServiceProxyProvider serviceProxyProvider,
            IServiceRouteProvider serviceRouteProvider)
        {
            _serviceProxyProvider = serviceProxyProvider;
            _serviceRouteProvider = serviceRouteProvider;
            _cacheProvider = CacheContainer.GetService<ICacheProvider>(GatewayConfig.CacheMode);
        }

        public async Task<string> GenerateTokenCredential(Dictionary<string, object> parameters)
        {
            var payload = await _serviceProxyProvider.Invoke<object>(parameters, GatewayConfig.AuthorizationRoutePath, GatewayConfig.AuthorizationServiceKey);
            if (payload == null || payload.Equals("null")) return null;
            var jwtHeader = JsonConvert.SerializeObject(new JwtSecureDataHeader
            {
                TimeStamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
            });
            var base64Payload = ConverBase64String(JsonConvert.SerializeObject(payload));
            var encodedString = $"{ConverBase64String(jwtHeader)}.{base64Payload}";
            var route = await _serviceRouteProvider.GetRouteByPath(GatewayConfig.AuthorizationRoutePath);
            var signature = HMACSHA256(encodedString, route.ServiceDescriptor.Token);
            var result = $"{encodedString}.{signature}";
            _cacheProvider.Add(base64Payload, result, GatewayConfig.AccessTokenExpireTimeSpan);
            return result;
        }

        public async Task<bool> ValidateClientAuthentication(string token)
        {
            bool isSuccess = false;
            var jwtToken = token.Split('.');
            if (jwtToken.Length == 3)
            {
                isSuccess = await _cacheProvider.GetAsync<string>(jwtToken[1]) == token;
            }

            return isSuccess;
        }

        public string GetPayloadString(string token)
        {
            string result = null;
            var jwtToken = token.Split('.');
            if (jwtToken.Length == 3)
            {
                result = Encoding.UTF8.GetString(Convert.FromBase64String(jwtToken[1]));
            }

            return result;
        }

        private string ConverBase64String(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        private string HMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var keyByte = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}