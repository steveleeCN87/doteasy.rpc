using doteasy.rpc.interfaces;
using DotEasy.Rpc.Core.DependencyResolver.Builder;

namespace doteasy.rpc.implement
{
    public class ProxyCommpoundService : RpcServiceBase, IProxyCommpoundService
    {
        public CompoundObject GetCurrentObject(CompoundObject parameter)
        {
            parameter.HashCdoe = parameter.GetHashCode();
            return parameter;
        }
    }
}