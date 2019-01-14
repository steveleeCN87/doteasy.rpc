using System;
using System.Collections.Generic;
using System.Threading;

namespace DotEasy.Rpc.Core.Cache.Caching
{
    public class CachingObjectPool<T>
    {
        #region 
        private int _isTaked;
        private Queue<T> queue = new Queue<T>();
        private Func<T> func;
        private int _currentResource;
        private int _tryNewObject;
        private readonly int _minSize = 1;
        private readonly int _maxSize = 50;
        #endregion

        #region private methods
        private void Enter()
        {
            while (Interlocked.Exchange(ref _isTaked, 1) != 0)
            {
            }
        }
        private void Leave()
        {
            Interlocked.Exchange(ref _isTaked, 0);
        }
        #endregion

        /// <summary>
        /// 构造一个对象池
        /// </summary>
        /// <param name="func">用来初始化对象的函数</param>
        /// <param name="minSize">对象池下限</param>
        /// <param name="maxSize">对象池上限</param>
        public CachingObjectPool(Func<T> func, int minSize = 100, int maxSize = 100)
        {
            Check.CheckCondition(() => func == null, "func");
            Check.CheckCondition(() => minSize < 0, "minSize");
            Check.CheckCondition(() => maxSize < 0, "maxSize");
            if (minSize > 0)
                _minSize = minSize;
            if (maxSize > 0)
                _maxSize = maxSize;
            for (var i = 0; i < _minSize; i++)
            {
                queue.Enqueue(func());
            }
            _currentResource = _minSize;
            _tryNewObject = _minSize;
            func = func;
        }

        /// <summary>
        /// 从对象池中取一个对象出来, 执行完成以后会自动将对象放回池中
        /// </summary>
        public T GetObject()
        {
            var t = default(T);
            try
            {
                if (_tryNewObject < _maxSize)
                {
                    Interlocked.Increment(ref _tryNewObject);
                    t = func();
                    // Interlocked.Increment(ref currentResource);
                }
                else
                {
                    Enter();
                    t = queue.Dequeue();
                    Leave();
                    Interlocked.Decrement(ref _currentResource);
                }
                return t;
            }
            finally
            {
                Enter();
                queue.Enqueue(t);
                Leave();
                Interlocked.Increment(ref _currentResource);
            }
        }

    }
}
