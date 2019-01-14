using System.Threading.Tasks;
using DotEasy.Rpc.Core.Cache.HashAlgorithms;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Address;

namespace DotEasy.Rpc.Core.Runtime.Client.Address.Resolvers
{
    /// <summary>
    /// 抽象的服务地址解析器
    /// </summary>
    public interface IAddressResolver
    {
        /// <summary>
        /// 解析服务地址
        /// </summary>
        /// <param name="serviceId">服务Id</param>
        /// <returns>服务地址模型</returns>
        Task<AddressModel> Resolver(string serviceId);
        
        
        ValueTask<ConsistentHashNode> Resolver(string cacheId, string item);
    }
}