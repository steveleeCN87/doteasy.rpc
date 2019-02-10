namespace doteasy.client
{
    internal static class Program
    {
        // RPC接口，不包含RPC通信验证
//        private static void Main() => RpcClient.TestNoToken();

        // RPC接口，包含RPC通信验证
//        private static void Main() => RpcClient.TestHaveToken();

        // 一个包含认证授权的客户端测试，需要API网关做中间转接
//        private static void Main() => AuthClient.TestHttpRoute();
        
        // 一个包含认证授权的客户端测试，需要API网关做中间转接，且服务节点采用rpc
        private static void Main() => AuthClient.TestHttpRouteRpc();
    }
}