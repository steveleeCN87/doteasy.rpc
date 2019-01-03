using System.Threading.Tasks;

namespace DotEasy.Rpc.Core.Routing
{
    public interface IServiceRouteProvider
    {
        Task<ServiceRoute> Locate(string serviceId);

        ValueTask<ServiceRoute> GetRouteByPath(string path);

        Task<ServiceRoute> SearchRoute(string path);
    }
}
