using System;
using DotEasy.Rpc.Core.Cache.Model;

namespace DotEasy.Rpc.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class IdentifyCacheAttribute : Attribute
    {
        public IdentifyCacheAttribute(CacheTargetType name)
        {
            Name = name;
        }

        public CacheTargetType Name { get; set; }
    }
}