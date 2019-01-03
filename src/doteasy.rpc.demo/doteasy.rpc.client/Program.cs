using System;
using System.Threading.Tasks;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Consul.Entry;

namespace doteasy.client
{
    class Program
    {
        static void Main()
        {
            new TestClient();
        }
    }

    public class TestClient : ClientBase
    {
        public TestClient()
        {
            Task.Run(async () =>
            {
                var userService = Proxy<IUserService>();
                Console.WriteLine($"{userService.GetDictionary().Result["key"]}");
                Console.WriteLine($"{await userService.Async(1)}");
                Console.WriteLine($"{userService.Sync(1)}");
            }).Wait();
            Console.ReadKey();
        }
    }
}