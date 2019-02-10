using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using DotEasy.Rpc.Core.Runtime.Client;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using Newtonsoft.Json;

namespace DotEasy.Rpc.Core.ApiGateway.Impl
{
    public class DefaultRelayHttpRouteRpc : IRelayHttpRouteRpc
    {
        private IRemoteInvokeService _remoteInvokeService;
        private ITypeConvertibleService _typeConvertibleService;

        public DefaultRelayHttpRouteRpc(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService)
        {
            _remoteInvokeService = remoteInvokeService;
            _typeConvertibleService = typeConvertibleService;
        }

        public StringContent HttpRouteRpc(List<dynamic> proxys, Uri urlPath, HttpRequestHeaders headers)
        {
            foreach (var proxy in proxys)
            {
                Type type = proxy.GetType();
                if (!urlPath.Query.Contains("scheme=rpc")) continue;

                var predicate = urlPath.AbsolutePath.Split('/');
                var absName = predicate[predicate.Length - 1];
                var absPars = predicate[predicate.Length - 2];

                if (!type.GetMethods().Any(methodInfo => methodInfo.Name.Contains(absName))) continue;

                var method = type.GetMethod(absName);
                if (method != null)
                {
                    var parameters = method.GetParameters();
                    var parType = parameters[0].ParameterType; // only one parameter
                    var par = _typeConvertibleService.Convert(absPars, parType);

                    var relayScriptor = new RelayScriptor {InvokeType = type, InvokeParameter = new dynamic[] {par}};

                    var result = method.Invoke(
                        Activator.CreateInstance(relayScriptor.InvokeType, _remoteInvokeService, _typeConvertibleService),
                        relayScriptor.InvokeParameter);

                    var strResult = JsonConvert.SerializeObject(result);
                    return new StringContent(strResult);
                }
            }

            return null;
        }
    }
}