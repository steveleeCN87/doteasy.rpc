using System;

namespace DotEasy.Rpc.Core.DependencyResolver
{
    public class ServiceBase
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}