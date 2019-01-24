using System.Collections.Generic;
using System.Threading.Tasks;
using DotEasy.Rpc.ApiGateway.AuthMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Middleware.Multiplexer;

namespace DotEasy.Rpc.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // 添加网关
            services.AddOcelot();
//                .AddTransientDefinedAggregator<FakeDefinedAggregator>();
            // 添加权限认证服务
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(TestConfig.GetApiResources())
                .AddInMemoryClients(TestConfig.GetClients())
                .AddTestUsers(TestConfig.GetUsers());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseIdentityServerToken();
            app.UseIdentityServer();
            app.UseOcelot().Wait();
            app.UseMvc();
        }
    }

    public class FakeDefinedAggregator : IDefinedAggregator 
    {
        public Task<DownstreamResponse> Aggregate(List<DownstreamResponse> responses)
        {
            throw new System.NotImplementedException();
        }
    }
}