using System.Threading.Tasks;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Core.Transport.Impl
{
    /// <summary>
    /// 消息监听者
    /// </summary>
    public class MessageListener : IMessageListener
    {
        /// <summary>
        /// 接收到消息的事件
        /// </summary>
        public event ReceivedDelegate Received;

        /// <summary>
        /// 触发接收到消息事件
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">接收到的消息</param>
        /// <returns>一个任务</returns>
        public async Task OnReceived(IMessageSender sender, TransportMessage message)
        {
            if (Received == null) return;
            await Received(sender, message);
        }
    }
}