using System;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Consul.Entry;

namespace doteasy.client
{
    class Program
    {
        private static void Main()
        {
            using (var proxy = ClientProxy.Generate<IProxyService>("http://127.0.0.1:8500"))
            {
                Console.WriteLine($"{proxy.GetDictionary().Result["key"]}");
                Console.WriteLine($"{proxy.Async(1).Result}");
                Console.WriteLine($"{proxy.Sync(1)}");
            }

            Console.ReadKey();
        }
    }
}