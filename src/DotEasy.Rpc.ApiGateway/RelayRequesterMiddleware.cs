using System;
using System.Net;
using System.Threading.Tasks;
using DotEasy.Rpc.Consul.Entry;
using DotEasy.Rpc.Core.ApiGateway;
using DotEasy.Rpc.Core.ApiGateway.Impl;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace DotEasy.Rpc.ApiGateway
{
    public class RelayRequesterMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;

        // ReSharper disable once NotAccessedField.Local
        private readonly IOcelotLogger _logger;

        public RelayRequesterMiddleware(OcelotRequestDelegate next, IOcelotLoggerFactory loggerFactory) : base(loggerFactory
            .CreateLogger<RelayRequesterMiddleware>())
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RelayRequesterMiddleware>();
        }

        /// <summary>
        /// 当下游协议是RPC时被调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(DownstreamContext context)
        {
            IRelayHttpRouteRpc relayHttpRouteRpc = new DefaultRelayHttpRouteRpc(ClientProxy.IRemoteInvokeService, ClientProxy.ITypeConvertibleService);

            var httpContent = relayHttpRouteRpc.HttpRouteRpc(
                ClientProxy.GenerateAll(new Uri("http://127.0.0.1:8500")),
                new Uri(context.DownstreamRequest.ToUri()),
                context.DownstreamRequest.Headers);

            context.DownstreamResponse = new DownstreamResponse(httpContent, HttpStatusCode.OK, context.DownstreamRequest.Headers);
            await _next.Invoke(context);
        }
    }
}