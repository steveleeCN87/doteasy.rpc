using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotEasy.Rpc.Runtime.Communally.Entitys
{
    /// <summary>
    /// 服务实体
    /// </summary>
    public class ServiceEntity
    {
        /// <summary>
        /// 执行委托
        /// </summary>
        public Func<IDictionary<string, object>, Task<object>> Func { get; set; }

        /// <summary>
        /// 服务描述符
        /// </summary>
        public ServiceDescriptor Descriptor { get; set; }
    }
}