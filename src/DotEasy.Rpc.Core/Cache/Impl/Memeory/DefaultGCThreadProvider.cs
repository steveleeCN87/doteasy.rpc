using System;
using System.Collections.Concurrent;
using System.Threading;
using DotEasy.Rpc.Core.Cache.Caching;

namespace DotEasy.Rpc.Core.Cache.Impl.Memeory
{
    public class DefaultGCThreadProvider
    {
        #region 私有字段
        /// <summary>
        /// 本地缓存垃圾回收线程
        /// </summary>
        private static readonly ConcurrentStack<ParameterizedThreadStart> _globalThread = new ConcurrentStack<ParameterizedThreadStart>();
        #endregion
        
        /// <summary>
        /// 添加垃圾线程方法
        /// </summary>
        /// <param name="start"> 表示在 System.Threading.Thread 上执行的方法。</param>
        public static void AddThread(ParameterizedThreadStart start)
        {
            AddThread(start, null);
        }

        /// <summary>
        /// 添加垃圾线程方法
        /// </summary>
        /// <param name="paramThreadstart">表示在 System.Threading.Thread 上执行的方法。</param>
        /// <param name="para">  包含该线程过程的数据的对象。</param>
        public static void AddThread(ParameterizedThreadStart paramThreadstart, object para)
        {
            ParameterizedThreadStart threadstart;
            try
            {
                if (!_globalThread.TryPop(out threadstart))
                {
                    _globalThread.Push(paramThreadstart);
                    Thread thread = new Thread(paramThreadstart);
                    thread.IsBackground = true;
                    thread.Start(para ?? thread.ManagedThreadId);
                }
            }
            catch (Exception err)
            {
                throw new CachingException(err.Message, err);
            }
        }
    }
}
