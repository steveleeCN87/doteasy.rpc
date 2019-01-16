using System;
using System.Net.Http;
using IdentityModel.Client;

namespace doteasy.client
{
    public static class AuthClient
    {
        /// <summary>
        /// 获取范文token
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            var disco = DiscoveryClient.GetAsync("http://127.0.0.1:8080").Result;
            TokenResponse tokenResponse;
            using (var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret"))
            {
                tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1").Result; //使用用户名密码
            }

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return "";
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// 根据TOKEN测试一个地址接口
        /// </summary>
        public static void TestAllDisco()
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetToken());
                // 通过网关访问具体资源
                // 此处访问的是网关的接口映射路径，而不是实际的接口URL路径
                response = httpClient.GetAsync(new Uri("http://127.0.0.1:8080/api/values")).Result; 
            }

            Console.WriteLine("response: " + response);
            Console.WriteLine("content: " + response.Content.ReadAsStringAsync().Result);
        }

        public static void TestTokenValidate()
        {
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjRhMjZjNDZlMzY0NjY2ODgwYjk0MGE1YjZmY2FkMCIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1NDc0MzMwOTEsImV4cCI6MTU0NzQzNjY5MSwiaXNzIjoiaHR0cDovLzEyNy4wLjAuMTo4MDgwIiwiYXVkIjpbImh0dHA6Ly8xMjcuMC4wLjE6ODA4MC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6InJvLmNsaWVudCIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1NDc0MzMwOTEsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsiYXBpMSJdLCJhbXIiOlsicHdkIl19.SmzE3KR_FOfIFIzjnAqiHVt35uefpuiExtnwKO9msIYl389bLjvLWqgwyRV5XgT0oIPYcvj2Th5ABBM9baD-pHCOaGooEwHYA4ydu1yabqEKLIooEV_mo73OSQHMYIo9DGzTddg8Ut7JKyVHLZAAJfz6NMp6NZwunEMrF1NsIj6GiL1psZ-kyZSrvIdUSFHh92mCjPmiUfUdUPZIlVZLYrFEsxJQ6gHgQUpMwwQscdoLXkyw6PJ6xLhW_RJvOYWMust1TIvMqVaxsouuaV6EKACpOJndSy7JuQy-_7Gbes7jYlrS-bntsLoi4SK9SDJenlHHc-lCUIbsHIDkbZEiwg";
            var jwtToken = token.Split('.');
            if (jwtToken.Length == 3)
            {
                /*
                 * TOKEN分三段
                 * 1段：Header请求头
                 * 2段：payload有效载荷
                 * 3段：Signature签名
                 */
                
            }
        }
    }
}