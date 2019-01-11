using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace doteasy.client
{
    public static class AuthClient
    {
        public static void Test()
        {
            // 从元数据中发现客户端
            var disco = DiscoveryClient.GetAsync("http://127.0.0.1:2000").Result;

            // 请求令牌
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1").Result; //使用用户名密码

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            
        }

        public static void TestAllDisco()
        {
            var httpClient = new HttpClient();
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Beaer","");
            
            var response = httpClient.GetAsync(new Uri("http://127.0.0.1:5000/api/values")).Result;
            Console.WriteLine("response: " + response);
            Console.WriteLine("content: " + response.Content.ReadAsStringAsync().Result);
        }
    }
}