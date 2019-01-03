using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotEasy.Rpc.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Runtime.Communally.Entitys;
using DotEasy.Rpc.Runtime.Communally.IdGenerator;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = DotEasy.Rpc.Runtime.Communally.Entitys.ServiceDescriptor;

namespace DotEasy.Rpc.Runtime.Server.Impl
{
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
            foreach (var methodInfo in service.GetTypeInfo().GetMethods())
            {
                // 获取方法名和参数名（MethodInfo）
                var implementationMethodInfo = serviceImplementation.GetTypeInfo().GetMethod(
                    methodInfo.Name,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray()
                );
                yield return Create(methodInfo, implementationMethodInfo);
            }
        }

        // 此方法严重依赖于Microsoft.Extensions.DependencyInjection库，需要拆分
        // 通过容器实例化ServiceEntity实体
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
                        var instance = scope.ServiceProvider.GetRequiredService(method.DeclaringType);

                        var list = new List<object>();
                        foreach (var parameterInfo in implementationMethod.GetParameters())
                        {
                            list.Add(
                                _typeConvertibleService.Convert(
                                    parameters[parameterInfo.Name],
                                    parameterInfo.ParameterType)
                            );
                        }

                        return Task.FromResult(implementationMethod.Invoke(
                            instance,
                            list.ToArray()
                        ));
                    }
                }
            };
        }
    }
}