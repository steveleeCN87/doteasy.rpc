using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using DotEasy.Rpc.Core.Communally.Exceptions;
using DotEasy.Rpc.Core.Server;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Transport.Impl
{
    /// <summary>
    /// 默认的传输客户端实现, 包含默认接收和方法发送
    /// </summary>
    public class TransportClient : ITransportClient, IDisposable
    {
        private readonly IMessageSender _messageSender;
        private readonly IMessageListener _messageListener;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>> _resultDictionary
            = new ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>>();

        public TransportClient(IMessageSender messageSender, IMessageListener messageListener, 
            IServiceExecutor serviceExecutor, ILogger logger)
        {
            var serviceExecutor1 = serviceExecutor;

            _messageSender = messageSender;
            _messageListener = messageListener;
            _logger = logger;

            messageListener.Received += async (sender, message) =>
            {
                if (!_resultDictionary.TryGetValue(message.Id, out var task))
                    return;

                if (message.IsInvokeResultMessage())
                {
                    var content = message.GetContent<RemoteInvokeResultMessage>();
                    if (!string.IsNullOrEmpty(content.ExceptionMessage))
                    {
                        task.TrySetException(new RpcRemoteException(content.ExceptionMessage));
                    }
                    else
                    {
                        task.SetResult(message);
                    }
                }

                if (serviceExecutor1 != null && message.IsInvokeMessage())
                    await serviceExecutor1.ExecuteAsync(sender, message);
            };
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">远程调用消息模型</param>
        /// <returns>远程调用消息的传输消息</returns>
        public async Task<RemoteInvokeResultMessage> SendAsync(RemoteInvokeMessage message)
        {
            try
            {
                
                    Console.WriteLine("准备发送消息。");

                var transportMessage = TransportMessage.CreateInvokeMessage(message);

                //注册结果回调
                var callbackTask = RegisterResultCallbackAsync(transportMessage.Id);

                try
                {
                    //发送
                    await _messageSender.SendAndFlushAsync(transportMessage);
                }
                catch (Exception exception)
                {
                    throw new RpcCommunicationException("与服务端通讯时发生了异常", exception);
                }

                
                    Console.WriteLine("消息发送成功。");

                return await callbackTask;
            }
            catch (Exception exception)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("消息发送失败。", exception);
                throw;
            }
        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            (_messageSender as IDisposable)?.Dispose();
            (_messageListener as IDisposable)?.Dispose();

            foreach (var taskCompletionSource in _resultDictionary.Values)
            {
                taskCompletionSource.TrySetCanceled();
            }
        }


        /// <summary>
        /// 注册指定消息的回调任务
        /// </summary>
        /// <param name="id">消息Id</param>
        /// <returns>远程调用结果消息模型</returns>
        private async Task<RemoteInvokeResultMessage> RegisterResultCallbackAsync(string id)
        {
            
                Console.WriteLine($"准备获取Id为：{id}的响应内容。");

            var task = new TaskCompletionSource<TransportMessage>();
            _resultDictionary.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result.GetContent<RemoteInvokeResultMessage>();
            }
            finally
            {
                //删除回调任务
//                TaskCompletionSource<TransportMessage> value;
                _resultDictionary.TryRemove(id, out _);
            }
        }
    }
}