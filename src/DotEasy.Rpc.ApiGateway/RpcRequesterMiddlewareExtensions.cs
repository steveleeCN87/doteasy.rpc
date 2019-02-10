using Ocelot.Middleware.Pipeline;

namespace DotEasy.Rpc.ApiGateway
{
    public static class RpcRequesterMiddlewareExtensions
    {
        public static void UseRpcRequesterMiddleware(this IOcelotPipelineBuilder builder)
        {
            builder.UseMiddleware<RelayRequesterMiddleware>();
        }
    }
}