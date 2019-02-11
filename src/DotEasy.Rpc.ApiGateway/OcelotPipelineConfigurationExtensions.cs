using System;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.RateLimit.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.RequestId.Middleware;

namespace DotEasy.Rpc.ApiGateway
{
    public static class OcelotPipelineConfigurationExtensions
    {
        public static void AddRpcMiddleware(this OcelotPipelineConfiguration config)
        {
            config.MapWhenOcelotPipeline.Add(builder => builder.AddRpcMiddleware());
        }

        private static Func<DownstreamContext, bool> AddRpcMiddleware(this IOcelotPipelineBuilder builder)
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

            // We fire off the request and set the response on the scoped data repo
            builder.UseRpcRequesterMiddleware();

            // This check the downstream route scheme is tcp or custom scheme, for example @rpc
            return context => context.DownstreamReRoute.DownstreamScheme.Equals("tcp", StringComparison.OrdinalIgnoreCase);
        }
    }
}