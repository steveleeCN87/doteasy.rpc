using Consul;

namespace DotEasy.Rpc.Consul
{
    public class ConsulRpcOptionsConfiguration
    {
        /// <summary> ASP.NET Core runtime hosting </summary>
        public string HostingUrls { get; set; } = "http://127.0.0.1:5000/";

        /// <summary> hosting and rpc (or consul) health check status </summary>
        public string HostingAndRpcHealthCheck { get; set; } = "http://127.0.0.1:5000/api/health";

        public Rpc GRpc { get; set; }

        public ServiceDescriptor ServiceDescriptors { get; set; }
        public ConsulRegister ConsulRegister { get; set; }
        public ConsulClientConfiguration ConsulClientConfiguration { get; set; }
    }

    /// <summary> Rpc remote invoke and Rpc hosting server address </summary>
    public class Rpc
    {
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 9881;
    }

    public class ServiceDescriptor
    {
        public string Name { get; set; } = "DotEasy.WebServer";
    }

    /// <summary> Register consul interface address </summary>
    public class ConsulRegister
    {
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8500;
        public int Timeout { get; set; } = 5;
    }
}