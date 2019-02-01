### DotEasy.RPC

[English](https://github.com/steveleeCN87/doteasy.rpc/blob/master/README.md)

![Image text](https://raw.githubusercontent.com/steveleeCN87/doteasy.rpc/master/%E5%BE%AE%E6%9C%8D%E5%8A%A1%E6%A1%86%E6%9E%B6.png)

![Image text](https://camo.githubusercontent.com/890acbdcb87868b382af9a4b1fac507b9659d9bf/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f6c6963656e73652d4d49542d626c75652e737667)

# 框架介绍
随着网站系统的不断发展，架构的复杂性将从MVC-> SOA->微服务，从简单到复杂，从集中到分布，面对服务的增加，服务分配的部署，服务和服务之间的相互呼叫，必须使用服务框架来解决，服务框架的引入是SOA->微服务过程必须解决的问题，本框架基于.NET Core 2.0 Standard 2开发，DotEasy.RPC支持从客户端到服务器的透明调用，就像对接口的实现调用一样简单。


# 依赖
1. 使用Microsoft.Extensions.Dependency Injection自动组装和构建相关的组件类型。
2. 流序列化使用 [protobuf-net](https://github.com/mgravell/protobuf-net)。
3. 动态代理生成使用 [Roslyn](https://github.com/dotnet/roslyn)。 
4. 通信管道和服务宿主使用 [DotNetty](https://github.com/Azure/DotNetty)。


# 更多
[http://www.cnblogs.com/SteveLee/](http://www.cnblogs.com/SteveLee/)


# 如何使用
在你的Web Api Core中添加如下代码，实现中间件扩展服务注入。
```
app.UseConsulServerExtensions(Configuration,
    collection =>
    {
        collection.AddSingleton<IProxyService, ProxyImpl>();
    },
    typeof(AuthorizationServerProvider)
);
```
在你的应用程序中，添加如下代码，即可实现客户端代理生成。 
```
using (var proxy = ClientProxy.Generate<IProxyService>(new Uri("http://127.0.0.1:8500")))
{
    Console.WriteLine($@"{proxy.Sync(1)}");
    Console.WriteLine($@"{proxy.Async(1).Result}");
    Console.WriteLine($@"{proxy.GetDictionaryAsync().Result["key"]}");
}
```
具体接口及接口实现由你自行定义，以上只是一个最简单的框架调用示例。


# 版本变化
## 1.0.3
1. 移除懒人使用包，极大的简化了客户端和服务端的调用方式，详见“如何使用”。
2. 修改客户端代理模式，结合ApiGateway（或者自定），实现OAuth2.0身份认证（JWT）。
3. 自动释放接口请求和实例类。

## 1.0.2
1. 添加了预编译的同步和异步远程调用方法，以符合代码设计规范。

## 1.0.1
1. 添加Consul注册和回调以实现Consul注册中心的配置。
2. 添加懒人使用包。