using System.Linq;
using System.Threading.Tasks;

namespace Easy.Rpc.Routing.Impl
{
    /// <summary>
    /// 服务路由管理者扩展方法
    /// </summary>
    public static class ServiceRouteManagerExtensions
    {
        /// <summary>
        /// 根据服务Id获取一个服务路由
        /// </summary>
        /// <param name="serviceRouteManager">服务路由管理者</param>
        /// <param name="serviceId">服务Id</param>
        /// <returns>服务路由</returns>
        public static async Task<ServiceRoute> GetAsync(this IServiceRouteManager serviceRouteManager, string serviceId)
        {
            return (await serviceRouteManager.GetRoutesAsync()).SingleOrDefault(i => i.ServiceDescriptor.Id == serviceId);
        }
    }
}