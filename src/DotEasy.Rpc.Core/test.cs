//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
//using DotEasy.Rpc.Core.Runtime.Client;
//using DotEasy.Rpc.Core.Runtime.Communally.Serialization;
//using DotEasy.Rpc.Core.Proxy.Impl;
//
//namespace Rpc.Common.ClientProxys
//{
//    public class ProxyServiceClientProxy : ServiceProxyBase
//    {
//        public ProxyServiceClientProxy(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService) : base(
//            remoteInvokeService, typeConvertibleService)
//        {
//        }
//
//        public async Task<System.String> Async(System.Int32 id)
//        {
//            return await InvokeAsync<System.String>(new Dictionary<string, object> {{"id", id}},
//                "doteasy.rpc.interfaces.IProxyService.Async_id");
//        }
//
//        public System.String Sync(System.Int32 id)
//        {
//            return Invoke<System.String>(new Dictionary<string, object> {{"id", id}}, "doteasy.rpc.interfaces.IProxyService.Sync_id");
//        }
//
//        public Dictionary<System.String, System.String> GetDictionary()
//        {
//            return Invoke < System.Collections.Generic.Dictionary<sintrg, object>>(Dictionary<string, string>,
//                       "doteasy.rpc.interfaces.IProxyService.GetDictionary");
//        }
//
//        public void Dispose()
//        {
//        }
//    }
//}