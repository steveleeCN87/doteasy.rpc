using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DotEasy.Rpc.Core.Proxy
{
    /// <summary>
    /// 一个抽象的服务代理生成器
    /// </summary>
    public interface IServiceProxyGenerater
    {
        
        /// <summary>
        /// 生成服务代理
        /// </summary>
        /// <param name="interfaceTypes">需要被代理的接口类型</param>
        /// <returns>服务代理实现</returns>
        IEnumerable<Type> GenerateProxys(IEnumerable<Type> interfaceTypes);
        
        /// <summary>
        /// 生成服务代理
        /// </summary>
        /// <param name="interfaceTypes">需要被代理的接口类型</param>
        /// <param name="accessToken">访问Token</param>
        /// <returns>服务代理实现</returns>
        IEnumerable<Type> GenerateProxys(IEnumerable<Type> interfaceTypes, string accessToken);

        /// <summary>
        /// 生成服务代理代码树
        /// </summary>
        /// <param name="interfaceType">需要被代理的接口类型</param>
        /// <returns>代码树</returns>
        SyntaxTree GenerateProxyTree(Type interfaceType);
    }
}