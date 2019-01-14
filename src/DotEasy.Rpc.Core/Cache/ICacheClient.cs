using System.Threading.Tasks;
using DotEasy.Rpc.Core.Cache.Caching;

namespace DotEasy.Rpc.Core.Cache
{
    /// <summary>
    /// 一个默认的缓存客户端
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICacheClient<T>
    {
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="info"></param>
        /// <param name="connectTimeout"></param>
        /// <returns></returns>
        T GetClient(CachingEndpoint info, int connectTimeout);

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="connectTimeout"></param>
        /// <returns></returns>
        Task<bool> ConnectionAsync(CachingEndpoint endpoint, int connectTimeout);
    }
}