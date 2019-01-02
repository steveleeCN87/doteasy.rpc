using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotEasy.Rpc.Proxy
{
   public interface  IServiceProxyProvider
    {
        Task<T> Invoke<T>(IDictionary<string, object> parameters, string routePath);

        Task<T> Invoke<T>(IDictionary<string, object> parameters, string routePath,string serviceKey);
    }
}
