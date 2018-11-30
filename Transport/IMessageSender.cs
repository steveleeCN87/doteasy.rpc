using System.Threading.Tasks;
using Easy.Rpc.Core.Communally.Entitys.Messages;

namespace Easy.Rpc.Transport
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