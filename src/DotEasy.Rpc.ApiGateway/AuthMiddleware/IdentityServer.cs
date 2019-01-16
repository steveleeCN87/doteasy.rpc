using Microsoft.AspNetCore.Builder;

namespace DotEasy.Rpc.ApiGateway.AuthMiddleware
{
    public static class IdentityServer
    {
        public static IApplicationBuilder UseIdentityServerToken(this IApplicationBuilder app)
        {
            app.UseMiddleware<TokenMiddleware>();
            return app;
        }
    }
}