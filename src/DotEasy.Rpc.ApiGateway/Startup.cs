using System;
using System.Threading.Tasks;
using DotEasy.Rpc.ApiGateway.AuthMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.RateLimit.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.RequestId.Middleware;

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
            app.UseOcelot(config => config.AddRpcGateway()).Wait();
            app.UseMvc();
        }
    }


    public static class OcelotPipelineConfigurationExtensions
    {
        public static void AddRpcGateway(this OcelotPipelineConfiguration config)
        {
            config.MapWhenOcelotPipeline.Add(builder => builder.AddRpcGateway(config));
        }

        private static Func<DownstreamContext, bool> AddRpcGateway(this IOcelotPipelineBuilder builder, OcelotPipelineConfiguration pipelineConfiguration)
        {
            builder.UseHttpHeadersTransformationMiddleware();

            // Initialises downstream request
            builder.UseDownstreamRequestInitialiser();

            // We check whether the request is ratelimit, and if there is no continue processing
            builder.UseRateLimiting();

            // This adds or updates the request id (initally we try and set this based on global config in the error handling middleware)
            // If anything was set at global level and we have a different setting at re route level the global stuff will be overwritten
            // This means you can get a scenario where you have a different request id from the first piece of middleware to the request id middleware.
            builder.UseRequestIdMiddleware();

            // This takes the downstream route we retrieved earlier and replaces any placeholders with the variables that should be used
            builder.UseDownstreamUrlCreatorMiddleware();

            //We fire off the request and set the response on the scoped data repo
            builder.UseRpcRequesterMiddleware();

            // This check the downstream route scheme is tcp or custom scheme, for example @rpc
//            return context => context.DownstreamReRoute.DownstreamScheme.Equals("http", StringComparison.OrdinalIgnoreCase);
            return context => context.DownstreamReRoute.DownstreamScheme.Equals("tcp", StringComparison.OrdinalIgnoreCase);
        }
    }


    public static class RpcRequesterMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseRpcRequesterMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<OrleansRequesterMiddleware>();
        }
    }

    public class OrleansRequesterMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IOcelotLogger _logger;

        public OrleansRequesterMiddleware(OcelotRequestDelegate next, IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<OrleansRequesterMiddleware>())
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<OrleansRequesterMiddleware>();
        }

        /// <summary>
        /// 当下游协议是RPC时被调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>需要配合DownstreamScheme实现</remarks>
        public async Task Invoke(DownstreamContext context)
        {
            /*
             * DownstreamContext 包含下游请求和响应的所有管道消息
             * 可以自定义下游协议的具体实现，比如将rpc的客户端代理用作实现
             * 再比如说可以将Consul作为服务发现，结合ocelot，自动路由下游信息
             */
            Console.WriteLine(context.DownstreamRequest.ToUri());
            _logger.LogInformation(context.DownstreamRequest.ToUri());
            
            
            await _next.Invoke(context);
        }
    }
}