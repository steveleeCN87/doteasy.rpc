namespace Easy.Rpc.Core.Communally.Entitys
{
    /// <summary>
    /// 服务描述符扩展方法
    /// </summary>
    public static class ServiceDescriptorExtensions
    {
        /// <summary>
        /// 获取组名称
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <returns>组名称</returns>
        public static string GroupName(this ServiceDescriptor descriptor)
        {
            return descriptor.GetMetadata<string>("GroupName");
        }

        /// <summary>
        /// 设置组名称
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <param name="groupName">组名称</param>
        /// <returns>服务描述符</returns>
        public static ServiceDescriptor GroupName(this ServiceDescriptor descriptor, string groupName)
        {
            descriptor.Metadatas["GroupName"] = groupName;
            return descriptor;
        }

        /// <summary>
        /// 设置是否等待执行
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <param name="waitExecution">如果需要等待执行则为true，否则为false，默认为true</param>
        /// <returns></returns>
        public static ServiceDescriptor WaitExecution(this ServiceDescriptor descriptor, bool waitExecution)
        {
            descriptor.Metadatas["WaitExecution"] = waitExecution;
            return descriptor;
        }

        /// <summary>
        /// 获取释放等待执行的设置
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <returns>如果需要等待执行则为true，否则为false，默认为true</returns>
        public static bool WaitExecution(this ServiceDescriptor descriptor)
        {
            return descriptor.GetMetadata("WaitExecution", true);
        }
    }
}