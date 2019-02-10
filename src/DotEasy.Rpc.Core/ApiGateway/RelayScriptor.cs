using System;

namespace DotEasy.Rpc.Core.ApiGateway
{
    public class RelayScriptor
    {
        public Type InvokeType { get; set; }
        public object[] InvokeParameter { get; set; }
    }
}