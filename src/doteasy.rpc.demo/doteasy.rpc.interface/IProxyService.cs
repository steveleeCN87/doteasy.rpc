using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Attributes;

namespace doteasy.rpc.interfaces
{
    [RpcTagBundle]
    public interface IProxyService : IDisposable
    {
        Task<IDictionary<string, string>> GetDictionaryAsync();
        Task<string> Async(int id);
        string Sync(int id);
    }
}