using System;
using System.Net;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Transport;

namespace DotEasy.Rpc.Core.Runtime.Server.Impl
{
    /// <summary>
    /// 默认的服务主机
    /// </summary>
    /// <remarks>继承于ServiceHostAbstract抽象类，重写StartAsync方法，此模式也可以自定用于外部DLL扩展</remarks>
    public class DefaultServiceHost : ServiceHostAbstract
    {
        private readonly Func<EndPoint, Task<IMessageListener>> _messageListenerFactory;
        private IMessageListener _serverMessageListener;

        public DefaultServiceHost(Func<EndPoint, Task<IMessageListener>> messageListenerFactory, IServiceExecutor serviceExecutor)
            : base(serviceExecutor)
        {
            _messageListenerFactory = messageListenerFactory;
        }

        public override void Dispose()
        {
            (_serverMessageListener as IDisposable)?.Dispose();
        }

        /// <summary>
        /// 启动主机
        /// </summary>
        /// <param name="endPoint">主机终结点</param>
        /// <returns>一个任务</returns>
        public override async Task StartAsync(EndPoint endPoint)
        {
            if (_serverMessageListener != null) return;
            _serverMessageListener = await _messageListenerFactory(endPoint);
            _serverMessageListener.Received += async (sender, message) =>
            {
                await Task.Run(() => { MessageListener.OnReceived(sender, message); });
            };
        }
    }
}