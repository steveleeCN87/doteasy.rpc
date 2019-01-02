using System.Threading.Tasks;
using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Transport
{
    /// <summary>
    //默认传输客户端
    /// </summary>
    public interface ITransportClient
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">远程调用消息模型</param>
        /// <returns>远程调用消息的传输消息</returns>
        Task<RemoteInvokeResultMessage> SendAsync(RemoteInvokeMessage message);
    }
}