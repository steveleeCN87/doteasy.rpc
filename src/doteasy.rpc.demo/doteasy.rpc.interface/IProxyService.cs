using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Attributes;

namespace doteasy.rpc.interfaces
{
    [RpcTagBundle]
    public interface IProxyService : IDisposable
    {
        string MultiParTest(string a, string b, string c);
        
//        Task<string> Async(int id);
//        string Sync(int id);
//        Task<IDictionary<string, string>> GetDictionary();

    }
}