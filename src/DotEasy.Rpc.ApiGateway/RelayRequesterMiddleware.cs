using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotEasy.Rpc.Consul.Entry;
using DotEasy.Rpc.Core.ApiGateway;
using DotEasy.Rpc.Core.ApiGateway.Impl;
using Newtonsoft.Json;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace DotEasy.Rpc.ApiGateway
{
    public class RelayRequesterMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IOcelotLogger _logger;

        public RelayRequesterMiddleware(OcelotRequestDelegate next, IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<RelayRequesterMiddleware>())
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
            
            StringContent httpContent = null;

            foreach (var proxy in ClientProxy.GenerateAll(new Uri("http://127.0.0.1:8500")))
            {
                Type type = proxy.GetType();
                if (!context.DownstreamRequest.Query.Contains("scheme=rpc")) continue;

                var predicate = context.DownstreamRequest.AbsolutePath.Split(@"/");
                var absName = predicate[predicate.Length - 1];
                var absPars = predicate[predicate.Length - 2];

                if (!type.GetMethods().Any(methodInfo => methodInfo.Name.Contains(absName))) continue;

                var method = type.GetMethod(absName);
                var parameters = method.GetParameters();
                var parType = parameters[0].ParameterType; // only one parameter
                var par = ClientProxy.ITypeConvertibleService.Convert(absPars, parType);

                var relayScriptor = new RelayScriptor {InvokeType = type, InvokeParameter = new dynamic[] {par}};

                var result = method.Invoke(
                    Activator.CreateInstance(relayScriptor.InvokeType, ClientProxy.IRemoteInvokeService, ClientProxy.ITypeConvertibleService),
                    relayScriptor.InvokeParameter);

                var strResult = JsonConvert.SerializeObject(result);
                _logger.LogInformation(strResult);
                httpContent = new StringContent(strResult);
                break;
            }

            context.DownstreamResponse = new DownstreamResponse(httpContent, HttpStatusCode.OK, context.DownstreamRequest.Headers);
            await _next.Invoke(context);
        }
        
        
    }
}