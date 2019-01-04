using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Attributes;

namespace doteasy.rpc.interfaces
{
    [RpcTagBundle]
    public interface IProxyService : IDisposable
    {
        Task<string> Async(int id);

        Task<IDictionary<string, string>> GetDictionary();

        string Sync(int id);
    }
}