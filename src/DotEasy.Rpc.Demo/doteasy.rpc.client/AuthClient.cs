using System;
using System.Net.Http;
using System.Net.Http.Headers;
using IdentityModel.Client;

namespace doteasy.client
{
    public static class AuthClient
    {
        public static string GetToken()
        {
            // 从元数据中发现客户端
            var disco = DiscoveryClient.GetAsync("http://127.0.0.1:2000").Result;

            // 请求令牌
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1").Result; //使用用户名密码

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return "";
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }

        public static void TestAllDisco()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            var response = httpClient.GetAsync(new Uri("http://127.0.0.1:5000/api/values")).Result;
            Console.WriteLine("response: " + response);
            Console.WriteLine("content: " + response.Content.ReadAsStringAsync().Result);
        }
    }
}