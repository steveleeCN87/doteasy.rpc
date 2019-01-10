using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys;
using DotEasy.Rpc.Core.Runtime.Communally.IdGenerator;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = DotEasy.Rpc.Core.Runtime.Communally.Entitys.ServiceDescriptor;

namespace DotEasy.Rpc.Core.Runtime.Server.Impl
{
    /// <summary>
    /// 服务执行工厂
    /// </summary>
    public class ServiceEntryFactory : IServiceEntryFactory
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 服务ID生成器
        /// </summary>
        private readonly IServiceIdGenerator _serviceIdGenerator;

        /// <summary>
        /// 类型转换器
        /// </summary>
        private readonly ITypeConvertibleService _typeConvertibleService;

        public ServiceEntryFactory(IServiceProvider serviceProvider, IServiceIdGenerator serviceIdGenerator,
            ITypeConvertibleService typeConvertibleService)
        {
            _serviceProvider = serviceProvider;
            _serviceIdGenerator = serviceIdGenerator;
            _typeConvertibleService = typeConvertibleService;
        }

        public IEnumerable<ServiceEntity> CreateServiceEntry(Type service, Type serviceImplementation)
        {
            return from methodInfo in service.GetTypeInfo().GetMethods()
                let implementationMethodInfo = serviceImplementation.GetTypeInfo().GetMethod(
                    methodInfo.Name,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray()
                )
                select Create(methodInfo, implementationMethodInfo);
        }

        /// <summary>
        /// 从容器中实例化对象，并调用对应方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="implementationMethod"></param>
        /// <returns>通过容器实例化ServiceEntity实体进行实例化，并调用相应的方法</returns>
        private ServiceEntity Create(MethodInfo method, MethodBase implementationMethod)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);
            var serviceDescriptor = new ServiceDescriptor {Id = serviceId};

            return new ServiceEntity
            {
                Descriptor = serviceDescriptor,
                Func = parameters =>
                {
                    // 从Microsoft.Extensions.DependencyInjection获取当前范围
                    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        return Task.FromResult(implementationMethod.Invoke(
                            scope.ServiceProvider.GetRequiredService(method.DeclaringType),
                            implementationMethod.GetParameters().Select(parameterInfo =>
                                _typeConvertibleService.Convert(parameters[parameterInfo.Name], parameterInfo.ParameterType)).ToArray()
                        ));
                    }
                }
            };
        }
    }
}