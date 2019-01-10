using doteasy.rpc.implement;
using doteasy.rpc.interfaces;
using DotEasy.Rpc.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace doteasy.rpc.webserver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            // 使用Consul服务发现
            app.UseConsulServerExtensions(Configuration, collection => { collection.AddSingleton<IProxyService, ProxyImpl>(); });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}