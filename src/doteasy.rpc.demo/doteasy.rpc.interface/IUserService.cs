using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotEasy.Rpc.Attributes;

namespace doteasy.rpc.interfaces
{
    [RpcTagBundle]
    public interface IUserService
    {
        Task<string> Async(int id);

        Task<IDictionary<string, string>> GetDictionary();

        string Sync(int id);
    }
}