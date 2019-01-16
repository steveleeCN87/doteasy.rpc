using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Core.Runtime.Client;
using DotEasy.Rpc.Core.Runtime.Communally.Serialization;
using DotEasy.Rpc.Core.Proxy.Impl;

namespace Rpc.Common.ClientProxys
{
    public class ProxyServiceClientProxy : ServiceProxyBase
    {
        public ProxyServiceClientProxy(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService) : base(
            remoteInvokeService, typeConvertibleService)
        {
        }

        public System.String MultiParTest(System.String a, System.String b, System.String c)
        {
            return Invoke<System.String>(
                new Dictionary<string, object>
                {
                    {"a", a},
                    {"b", b},
                    {"c", c},
                    {
                        "token",
                        "eyJhbGciOiJSUzI1NiIsImtpZCI6IjRhMjZjNDZlMzY0NjY2ODgwYjk0MGE1YjZmY2FkMCIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1NDc0MzMwOTEsImV4cCI6MTU0NzQzNjY5MSwiaXNzIjoiaHR0cDovLzEyNy4wLjAuMTo4MDgwIiwiYXVkIjpbImh0dHA6Ly8xMjcuMC4wLjE6ODA4MC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6InJvLmNsaWVudCIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1NDc0MzMwOTEsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsiYXBpMSJdLCJhbXIiOlsicHdkIl19.SmzE3KR_FOfIFIzjnAqiHVt35uefpuiExtnwKO9msIYl389bLjvLWqgwyRV5XgT0oIPYcvj2Th5ABBM9baD-pHCOaGooEwHYA4ydu1yabqEKLIooEV_mo73OSQHMYIo9DGzTddg8Ut7JKyVHLZAAJfz6NMp6NZwunEMrF1NsIj6GiL1psZ-kyZSrvIdUSFHh92mCjPmiUfUdUPZIlVZLYrFEsxJQ6gHgQUpMwwQscdoLXkyw6PJ6xLhW_RJvOYWMust1TIvMqVaxsouuaV6EKACpOJndSy7JuQy-_7Gbes7jYlrS-bntsLoi4SK9SDJenlHHc-lCUIbsHIDkbZEiwg"
                    }
                },
                "doteasy.rpc.interfaces.IProxyService.MultiParTest_a_b_c");
        }

        public void Dispose()
        {
        }
    }
}