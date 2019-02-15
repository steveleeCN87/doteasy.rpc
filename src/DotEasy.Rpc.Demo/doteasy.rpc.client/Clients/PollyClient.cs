/*
 * https://github.com/App-vNext/Polly/wiki
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Timeout;
using Polly.Caching.Memory;

namespace doteasy.client.Clients
{
    public static class PollyClient
    {
        public static void Retry()
        {
            var tick = 0;
            const int maxRetry = 6;
            var retry = Policy.Handle<Exception>().Retry(maxRetry);

            try
            {
                retry.Execute(() =>
                {
                    Console.WriteLine($@"try {++tick}");
                    if (tick >= 1)
                        // 出现故障，开始重试Execute
                        throw new Exception("throw the exception");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"exception : " + ex.Message);
            }
        }

        public static void CircuitBreaker()
        {
            var tick = 0;
            const int interval = 10;
            const int maxRetry = 6;
            var circuitBreaker = Policy.Handle<Exception>().CircuitBreaker(maxRetry, TimeSpan.FromSeconds(interval));

            while (true)
            {
                try
                {
                    circuitBreaker.Execute(() =>
                    {
                        Console.WriteLine($@"try {++tick}");
                        throw new Exception("throw the exception");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"exception : " + ex.Message);

                    // 当重试次数达到断路器指定的次数时，Polly会抛出The circuit is now open and is not allowing calls. 断路器已打开，不允许访问
                    // 为了演示，故意将下面语句写上，可退出while循环
                    // 实际环境中视情况，断开绝不等于退出，或许20-30秒后，服务维护后变得可用了
                    if (ex.Message.Contains("The circuit is now open and is not allowing calls"))
                    {
                        break;
                    }
                }

                Thread.Sleep(300);
            }
        }

        public static void Timeout()
        {
            const int timeoutSecond = 3;

            try
            {
                Policy.Wrap(
                    Policy.Timeout(timeoutSecond, TimeoutStrategy.Pessimistic),
                    Policy.Handle<TimeoutRejectedException>().Fallback(() => { })
                ).Execute(() =>
                {
                    Console.WriteLine(@"try");
                    Thread.Sleep(5000);
                });
            }
            catch (Exception ex)
            {
                // 当超时时间到，会抛出The delegate executed through TimeoutPolicy did not complete within the timeout.
                // 委托执行未在指定时间内完成
                Console.WriteLine($@"exception : {ex.GetType()} : {ex.Message}");
            }
        }

        public static void Cache()
        {
            const int ttl = 60;
            var policy = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromSeconds(ttl));

            var frist = policy.Execute(
                context => new Context("success", new Dictionary<string, object> {{"1", "1"}}),
                new Context("Frist", new Dictionary<string, object> {{"exception", "1"}})
            );

            var second = policy.Execute(
                context => new Context("success", new Dictionary<string, object> {{"1", "2"}}),
                new Context("Second", new Dictionary<string, object> {{"exception", "1"}})
            );

            Console.WriteLine(frist?.Values.ToList()[0] + @" : " + second?.Values.ToList()[0]);
        }

        public static void Fallback()
        {
            Policy.Handle<ArgumentException>().Fallback(() => { Console.WriteLine(@"error occured"); })
                .Execute(() =>
                {
                    Console.WriteLine(@"try");
                    // 出现故障，进行降级处理Fallback
                    throw new ArgumentException(@"throw the exception");
                });
        }
    }
}