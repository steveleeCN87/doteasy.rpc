using DotEasy.Rpc.Core.Communally.Entitys;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Core.Server
{
    /// <summary>
    //抽象的服务条目定位器
    /// </summary>
    public interface IServiceEntryLocate
    {
        /// <summary>
        /// 定位服务条目
        /// </summary>
        /// <param name="invokeMessage">远程调用消息</param>
        /// <returns>服务条目</returns>
        ServiceEntity Locate(RemoteInvokeMessage invokeMessage);
    }
}