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
        CompoundObject GetCurrentObject(CompoundObject parameter);
    }

    public class CompoundObject
    {
        public int? HashCdoe { get; set; }
        public long CurrentId { get; set; } = Int64.MaxValue;
        public string CompoundId { get; set; } = Guid.NewGuid().ToString("B");
        public DateTime? CurrentDateTime { get; set; }
        public CompoundType CompoundType { get; set; } = CompoundType.Multiple;
        public IEnumerable<string> CompoundList { get; set; } = new[] {Guid.NewGuid().ToString("P"), null};
    }

    public enum CompoundType
    {
        Simple,
        Multiple
    }
}