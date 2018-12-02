using System.Threading.Tasks;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using DotEasy.Rpc.Transport;

namespace DotEasy.Rpc.Core.Server
{
    /// <summary>
    //抽象的服务执行器
    /// </summary>
    public interface IServiceExecutor
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">调用消息</param>
        Task ExecuteAsync(IMessageSender sender,TransportMessage message);
    }
}