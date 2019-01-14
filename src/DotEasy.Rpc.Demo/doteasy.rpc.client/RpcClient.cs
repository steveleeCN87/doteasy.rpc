using System;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Consul.Entry;

namespace doteasy.client
{
    public static class RpcClient
    {
        public static void Test()
        {
            const string token =
                "eyJhbGciOiJSUzI1NiIsImtpZCI6IjRhMjZjNDZlMzY0NjY2ODgwYjk0MGE1YjZmY2FkMCIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1NDc0MzMwOTEsImV4cCI6MTU0NzQzNjY5MSwiaXNzIjoiaHR0cDovLzEyNy4wLjAuMTo4MDgwIiwiYXVkIjpbImh0dHA6Ly8xMjcuMC4wLjE6ODA4MC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6InJvLmNsaWVudCIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1NDc0MzMwOTEsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsiYXBpMSJdLCJhbXIiOlsicHdkIl19.SmzE3KR_FOfIFIzjnAqiHVt35uefpuiExtnwKO9msIYl389bLjvLWqgwyRV5XgT0oIPYcvj2Th5ABBM9baD-pHCOaGooEwHYA4ydu1yabqEKLIooEV_mo73OSQHMYIo9DGzTddg8Ut7JKyVHLZAAJfz6NMp6NZwunEMrF1NsIj6GiL1psZ-kyZSrvIdUSFHh92mCjPmiUfUdUPZIlVZLYrFEsxJQ6gHgQUpMwwQscdoLXkyw6PJ6xLhW_RJvOYWMust1TIvMqVaxsouuaV6EKACpOJndSy7JuQy-_7Gbes7jYlrS-bntsLoi4SK9SDJenlHHc-lCUIbsHIDkbZEiwg";

            using (var proxy = ClientProxy.Generate<IProxyService>(new Uri("http://127.0.0.1:8500"), token))
            {
                Console.WriteLine($@"{proxy.MultiParTest("aaaa", "bbb", "ccccc")}");
//                Console.WriteLine($@"{proxy.GetDictionary().Result["key"]}");
//                Console.WriteLine($@"{proxy.Async(1).Result}");
//                Console.WriteLine($@"{proxy.Sync(1)}");
            }

            Console.ReadKey();
        }
    }
}