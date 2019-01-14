using System;

namespace DotEasy.Rpc.Core.DependencyResolver.Builder
{
    public class RpcServiceBase
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}