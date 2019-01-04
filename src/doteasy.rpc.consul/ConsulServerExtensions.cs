using System;
using DotEasy.Rpc.Consul.Entry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace DotEasy.Rpc.Consul
{
    public static class ConsulServerExtensions
    {
        public static void UseConsulServerExtensions(this IApplicationBuilder app, IConfiguration configuration, ServerBase.RegisterEventHandler serviceCollection)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            
            var serverBase = new ServerBase(configuration);
            serverBase.RegisterEvent += serviceCollection;
            serverBase.Start();
        }
    }
}