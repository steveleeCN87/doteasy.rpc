namespace Easy.Rpc.Routing.Impl
{
    /// <summary>
    /// 服务路由事件参数
    /// </summary>
    public class ServiceRouteEventArgs
    {
        public ServiceRouteEventArgs(ServiceRoute route)
        {
            Route = route;
        }

        /// <summary>
        /// 服务路由信息
        /// </summary>
        public ServiceRoute Route { get; private set; }
    }
}