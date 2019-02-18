using System;
using System.Net.Http;
using Ocelot.Logging;
using Polly;
using Polly.Timeout;

namespace DotEasy.Rpc.ApiGateway
{
    public class CircuitBreakingDelegatingHandler : DelegatingHandler
    {
        private readonly IOcelotLogger _logger;
        private readonly int _exceptionsAllowedBeforeBreaking;
        private readonly TimeSpan _durationOfBreak;
        private readonly Policy _circuitBreakerPolicy;
        private readonly TimeoutPolicy _timeoutPolicy;

        public CircuitBreakingDelegatingHandler(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak, TimeSpan timeoutValue,
            TimeoutStrategy timeoutStrategy, IOcelotLogger logger, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }
    }
}