using Easy.Rpc.Core.Communally.Entitys.Messages;

namespace Easy.Rpc.Core.Client
{
    /// <summary>
    /// 远程调用上下文
    /// </summary>
    public class RemoteInvokeContext
    {
        /// <summary>
        /// 远程调用消息
        /// </summary>
        public RemoteInvokeMessage InvokeMessage { get; set; }
    }
}