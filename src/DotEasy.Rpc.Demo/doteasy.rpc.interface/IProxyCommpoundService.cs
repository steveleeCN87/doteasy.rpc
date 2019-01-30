using System;
using System.Collections.Generic;
using DotEasy.Rpc.Core.Attributes;

namespace doteasy.rpc.interfaces
{
    [RpcTagBundle]
    public interface IProxyCommpoundService : IDisposable
    {
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