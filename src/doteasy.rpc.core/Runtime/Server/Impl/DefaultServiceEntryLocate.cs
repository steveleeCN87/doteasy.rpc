using System.Linq;
using DotEasy.Rpc.Runtime.Communally.Entitys;
using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Runtime.Server.Impl
{
    /// <summary>
    /// 默认的服务条目定位器
    /// </summary>
    public class DefaultServiceEntryLocate : IServiceEntryLocate
    {
        private readonly IServiceEntryManager _serviceEntryManager;

        public DefaultServiceEntryLocate(IServiceEntryManager serviceEntryManager)
        {
            _serviceEntryManager = serviceEntryManager;
        }

        #region Implementation of IServiceEntryLocate

        /// <summary>
        /// 定位服务条目
        /// </summary>
        /// <param name="invokeMessage">远程调用消息</param>
        /// <returns>服务条目</returns>
        public ServiceEntity Locate(RemoteInvokeMessage invokeMessage)
        {
            var serviceEntries = _serviceEntryManager.GetEntries();
            /*
            Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists;
            this method throws an exception if more than one element satisfies the condition.
            返回序列中满足指定条件的唯一元素，如果没有此类元素，则返回默认值；如果超过一个元素满足条件，则此方法抛出异常
            根据服务ID和调用消息中的服务Id定位当前服务.
            */

            return serviceEntries.SingleOrDefault(entity => entity.Descriptor.Id == invokeMessage.ServiceId);
        }

        #endregion Implementation of IServiceEntryLocate
    }
}