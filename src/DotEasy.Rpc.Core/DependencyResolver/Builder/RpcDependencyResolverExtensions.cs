﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DotEasy.Rpc.Core.DependencyResolver.Builder
{
    /// <summary>
    /// 扩展依赖注入IOC容器 DotEasy.Rpc主要入口
    /// </summary>
    public static class RpcDependencyResolverExtensions
    {
        /// <summary>
        /// 通过KEY获取<see cref="T"/>实例
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="resolver">IOC对象容器</param>
        /// <param name="key">键</param>
        /// <returns>返回<see cref="T"/>实例</returns>
        public static T GetService<T>(this IDependencyResolver resolver, object key)
        {
            return (T) resolver.GetService(typeof(T), key);
        }

        /// <summary>
        /// 获取<see cref="T"/>实例
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="resolver">IOC对象容器</param>
        /// <returns>返回<see cref="T"/>实例</returns>
        public static T GetService<T>(this IDependencyResolver resolver)
        {
            return (T) resolver.GetService(typeof(T), null);
        }

        /// <summary>
        /// 通过类型获取对象
        /// </summary>
        /// <param name="resolver">IOC对象容器</param>
        /// <param name="type">类型</param>
        /// <returns>返回对象</returns>
        public static object GetService(this IDependencyResolver resolver, Type type)
        {
            return resolver.GetService(type, null);
        }

        /// <summary>
        /// 通过KEY获取<see cref="T"/>集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="resolver">IOC对象容器</param>
        /// <param name="key">键</param>
        /// <returns>返回<see cref="T"/>实例</returns>
        public static IEnumerable<T> GetServices<T>(this IDependencyResolver resolver, object key)
        {
            return resolver.GetServices(typeof(T), key).OfType<T>();
        }

        /// <summary>
        /// 获取<see cref="T"/>集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="resolver">IOC对象容器</param>
        /// <returns>返回<see cref="T"/>集合</returns>
        public static IEnumerable<T> GetServices<T>(this IDependencyResolver resolver)
        {
            return resolver.GetServices(typeof(T), null).OfType<T>();
        }

        /// <summary>
        /// 通过类型获取对象集合
        /// </summary>
        /// <param name="resolver">IOC对象容器</param>
        /// <param name="type">类型</param>
        /// <returns>返回集合</returns>
        public static IEnumerable<object> GetServices(this IDependencyResolver resolver, Type type)
        {
            return resolver.GetServices(type, null);
        }

        /// <summary>
        /// 通过KEY和TYPE获取实例对象集合
        /// </summary>
        /// <param name="resolver">IOC对象容器</param>
        /// <param name="type">类型</param>
        /// <param name="key">键</param>
        /// <returns>返回实例对象集合</returns>
        internal static IEnumerable<object> GetServiceAsServices(this IDependencyResolver resolver, Type type,
            object key)
        {
            var service = resolver.GetService(type, key);
            return service == null ? Enumerable.Empty<object>() : new[] {service};
        }
    }
}