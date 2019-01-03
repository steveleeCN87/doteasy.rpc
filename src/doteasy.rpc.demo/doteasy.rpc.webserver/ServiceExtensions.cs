using System;
using DotEasy.Rpc.Consul.Entry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace doteasy.rpc.webserver
{
    public static class ConsulServerExtensions
    {
        public static IApplicationBuilder UseConsulServerExtensions(this IApplicationBuilder app, IConfiguration configuration, ServerBase.RegisterEventHandler serviceCollection)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            var baseServer = new ServerBase(configuration);
            baseServer.RegisterEvent += serviceCollection;
            baseServer.Start();
            return app;
        }
    }
}