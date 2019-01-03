using System;
using System.Threading.Tasks;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Entry;

namespace DotEasy.Client
{
    class Program : BaseClient
    {
        static void Main()
        {
            new TestClient();
        }
    }

    public class TestClient : BaseClient
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