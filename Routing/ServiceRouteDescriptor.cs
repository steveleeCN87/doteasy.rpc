using System.Collections.Generic;
using Easy.Rpc.Core.Communally.Entitys;

namespace Easy.Rpc.Routing
{
    /// <summary>
    /// 服务路由描述符
    /// </summary>
    public class ServiceRouteDescriptor
    {
        /// <summary>
        /// 服务地址描述符集合
        /// </summary>
        public IEnumerable<ServiceAddressDescriptor> AddressDescriptors { get; set; }

        /// <summary>
        /// 服务描述符
        /// </summary>
        public ServiceDescriptor ServiceDescriptor { get; set; }
    }
}