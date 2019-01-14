using DotEasy.Rpc.Core.Cache.Caching;

namespace DotEasy.Rpc.Core.Cache.Impl.Redis
{
    /// <summary>
    /// redis 终端
    /// </summary>
    public class DefaultRedisEndpoint : CachingEndpoint
    {
        /// <summary>
        /// 主机
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public new string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public new int Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public int DbIndex { get; set; }

        public int MaxSize { get; set; }

        public int MinSize { get; set; }


        public override string ToString()
        {
            return string.Concat(new string[] {Host, ":", Port.ToString(), "::", DbIndex.ToString()});
        }
    }
}