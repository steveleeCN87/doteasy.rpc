using System;
using System.Collections.Generic;
using System.Linq;

namespace DotEasy.Rpc.Runtime.Communally.Entitys
{
    /// <summary>
    /// 服务描述符
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// 初始化一个新的服务描述符
        /// </summary>
        public ServiceDescriptor()
        {
            Metadatas = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 服务Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 访问的令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string RoutePath { get; set; }
        
        /// <summary>
        /// 元数据
        /// </summary>
        public IDictionary<string, object> Metadatas { get; set; }

        /// <summary>
        /// 获取一个元数据
        /// </summary>
        /// <typeparam name="T">元数据类型</typeparam>
        /// <param name="name">元数据名称</param>
        /// <param name="def">如果指定名称的元数据不存在则返回这个参数</param>
        /// <returns>元数据值</returns>
        public T GetMetadata<T>(string name, T def = default(T))
        {
            if (!Metadatas.ContainsKey(name)) return def;
            return (T) Metadatas[name];
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为true；否则，为false</returns>
        public override bool Equals(object obj)
        {
            var model = obj as ServiceDescriptor;
            if (model == null) return false;
            if (obj.GetType() != GetType()) return false;
            if (model.Id != Id) return false;

            return model.Metadatas.Count == Metadatas.Count && model.Metadatas.All(metadata =>
            {
                if (!Metadatas.TryGetValue(metadata.Key, out var value)) return false;
                if (metadata.Value == null && value == null) return true;
                if (metadata.Value == null || value == null) return false;

                return metadata.Value.Equals(value);
            });
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(ServiceDescriptor model1, ServiceDescriptor model2)
        {
            return Equals(model1, model2);
        }

        public static bool operator !=(ServiceDescriptor model1, ServiceDescriptor model2)
        {
            return !Equals(model1, model2);
        }
    }
}