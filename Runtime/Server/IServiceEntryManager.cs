using System.Collections.Generic;
using DotEasy.Rpc.Runtime.Communally.Entitys;

namespace DotEasy.Rpc.Runtime.Server
{
    /// <summary>
    /// 抽象的服务条目管理者
    /// </summary>
    public interface IServiceEntryManager
    {
        /// <summary>
        /// 获取服务条目集合
        /// </summary>
        /// <returns>服务条目集合</returns>
        IEnumerable<ServiceEntity> GetEntries();
    }
}