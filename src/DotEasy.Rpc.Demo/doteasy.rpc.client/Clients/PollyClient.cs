/*
 * https://github.com/App-vNext/Polly/wiki
 */

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Timeout;
using Polly.Caching.Memory;

namespace doteasy.client.Clients
{
    public static class PollyClient
    {
        public static void Define()
        {
            /*
             * 故障定义
             * https://github.com/App-vNext/Polly#step-1b-optionally-specify-return-results-you-want-to-handle
             * 常见故障定义方式是指定委托执行过程中出现的特定异常
             */
            // 特定异常
            Policy.Handle<DivideByZeroException>();
            // 条件异常
            Policy.Handle<ArgumentException>(ex => ex.HResult == 9999);
            // 多种异常
            Policy.Handle<DivideByZeroException>().Or<ArgumentException>();
            // 聚合异常
            Policy.Handle<ArgumentException>().Or<ArgumentException>();

            /*
             * 定义返回结果
             * https://github.com/App-vNext/Polly#step-1b-optionally-specify-return-results-you-want-to-handle
             * 指定要处理的返回结果
             */
            // 处理带条件的返回值
            Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound);
            // 处理多个条件的返回值
            Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
                .OrResult(r => r.StatusCode == HttpStatusCode.BadGateway);
            // 结果判断
            Policy.HandleResult<int>(ret => ret <= 0);

            /*
             * 故障处理策略：重试（Retry）
             * https://github.com/App-vNext/Polly#step-2--specify-how-the-policy-should-handle-those-faults
             * 指定策略应如何处理这些错误，常见的处理策略是重试
             */
            // 重试1次
            Policy.Handle<TimeoutException>().Retry();
            // 重试3次
            Policy.Handle<TimeoutException>().Retry(3);
            // 无限重试
            Policy.Handle<TimeoutException>().RetryForever();
            // 重试多次，每次重试都调用一个操作
            Policy.Handle<TimeoutException>().Retry(3, (exception, retryCount) =>
            {
                // do something 
            });
            // 重试固定时间间隔
            Policy.Handle<TimeoutException>().WaitAndRetry(3, _ => TimeSpan.FromSeconds(3));
            // 重试指定时间时间
            Policy.Handle<TimeoutException>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
            }, (exception, timeSpan, retryCount, context) =>
            {
                // do something
            });


            /*
             * 故障处理策略：回退（Fallback）
             * https://github.com/App-vNext/Polly#fallback
             * Fallback策略是在遇到故障是指定一个默认的返回值
             */
            // 返回一个值
            Policy<int>.Handle<TimeoutException>().Fallback(99);
            Policy<int>.Handle<TimeoutException>().Fallback(() => 99);
            // 或将返回值定义为一个方法
            Policy.Handle<TimeoutException>().Fallback(() => { });


            /*
             * 故障处理策略：断路保护（Circuit Breaker）
             * https://github.com/App-vNext/Polly#circuit-breaker
             * Circuit Breaker也是一种比较常见的处理策略，它可以指定一定时间内最大的故障发生次数，当超过了该故障次数时，在该时间段内，不再执行Policy内的委托操作。
             */
            // 在指定的连续异常数后断开，并在指定的持续时间内保持断开。
            Policy.Handle<TimeoutException>().CircuitBreaker(2, TimeSpan.FromMinutes(1));
            // 在指定的连续异常数后断开，并在规定的时间内保持电路断开，且调用一个改变状态的操作。
            var circuitBreaker = Policy.Handle<TimeoutException>().CircuitBreaker(2, TimeSpan.FromMinutes(1),
                (exception, timespan) =>
                {
                    // On Break
                },
                () =>
                {
                    // On Reset
                });
            /*
             Closed - 常态，可执行actions。
             Open - 自动控制器已断开，不允许执行actions。
             HalfOpen - 在自动断路时间到时，从断开的状态复原。
             Isolated - 在断开的状态时手动hold住，不允许执行actions。
             */
            var unused = circuitBreaker.CircuitState;
            // 除了超时和策略执行失败的这种自动方式外，也可以手动控制它的状态：
            // 手动打开(且保持)一个断路器，例如手动隔离downstream服务
            circuitBreaker.Isolate();
            // 重置一个断路器回closed的状态，可再次接受actions的执行
            circuitBreaker.Reset();


            /*
             * 故障处理策略：策略包装（弹性策略封装）
             * https://github.com/App-vNext/Polly#policywrap
             * 我们可以通过PWrap的方式（Polly的策略的封装属于弹性策略），封装出一个更加强大的自定义策略。
             */
            var fallback = Policy<int>.Handle<TimeoutException>().Fallback(100);
            var retry = Policy<int>.Handle<TimeoutException>().Retry(2);
            var policyWrap = Policy.Wrap(fallback, retry);
            policyWrap.Execute(() => { return 0; });
            // 超时策略用于控制委托的运行时间，如果达到指定时间还没有运行，则触发超时异常。
            Policy.Timeout(TimeSpan.FromSeconds(3), TimeoutStrategy.Pessimistic);
            /*
             Polly支持两种超时策略：
                Pessimistic： 悲观模式
                当委托到达指定时间没有返回时，不继续等待委托完成，并抛超时TimeoutRejectedException异常。
                Optimistic：乐观模式
                这个模式依赖于 co-operative cancellation，只是触发CancellationTokenSource.Cancel函数，需要等待委托自行终止操作。
                其中悲观模式比较容易使用，因为它不需要在委托额外的操作，但由于它本身无法控制委托的运行，函数本身并不知道自己被外围策略取消了，也无法在超时的时候中断后续行为。因此用起来反而还不是那么实用。
             */
            // 无操作策略(NoOp)，啥也不不干
            Policy.NoOp();
            // 舱壁隔离（Bulkhead Isolation）
            // 舱壁隔离是一种并发控制的行为，并发控制是一个比较常见的模式，Polly也提供了这方面的支持
            Policy.Bulkhead(12);
            // 超过了并发数的任务会抛BulkheadRejectedException，如果要放在队列中等待
            // 这种方式下，有12个并发任务，每个任务维持着一个并发队列，每个队列可以自持最大100个任务。
            Policy.Bulkhead(12, 100);
        }

        /// <summary>
        /// 重试 
        /// </summary>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 断路
        /// </summary>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 超时
        /// </summary>
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

        /// <summary>
        /// 缓存
        /// </summary>
        public static void Cache()
        {
            const int ttl = 60;
            var policy = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromSeconds(ttl));
            var context = new Context(operationKey: "cache_key");
            for (var i = 0; i < 3; i++)
            {
                var cache = policy.Execute(_ =>
                {
                    Console.WriteLine(@"get value");
                    return 3;
                }, context);
                Console.WriteLine(cache);
            }
        }

        /// <summary>
        /// 回退
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
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