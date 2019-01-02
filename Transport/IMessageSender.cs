using System.Threading.Tasks;
using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Transport
{
    /// <summary>
    /// 抽象的发送者
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        Task SendAsync(TransportMessage message);

        /// <summary>
        /// 发送消息并清空缓冲区
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        Task SendAndFlushAsync(TransportMessage message);
    }
}