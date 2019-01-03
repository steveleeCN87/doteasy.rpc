using System;
using Autofac;
using doteasy.rpc.implement;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Entry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace doteasy.rpc.webserver
{
    public static class ConsulServerExtensions
    {
        public static IApplicationBuilder UseConsulServerExtensions(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            BaseServer baseServer = new BaseServer(configuration);
            baseServer.RegisterEvent += collection => collection.RegisterType<UserService>().As<IUserService>();
            baseServer.Start();
            return app;
        }
    }
}