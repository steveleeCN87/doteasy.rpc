using System;
using DotEasy.Rpc.Cache.Model;

namespace DotEasy.Rpc.Attributes
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