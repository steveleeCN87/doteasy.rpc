using System.Collections.Generic;

namespace DotEasy.Rpc.Consul
{
    public class ConsulServiceMetaInfo
    {
        public string ID { get; set; }

        public string Node { get; set; }

        public string Address { get; set; }

        public string Datacenter { get; set; }

        public TaggedAddresses TaggedAddresses { get; set; }

        public Dictionary<string, string> NodeMeta { get; set; }

        public string ServiceKind { get; set; }

        public string ServiceID { get; set; }

        public string ServiceName { get; set; }

        public List<string> ServiceTags { get; set; }

        public string ServiceAddress { get; set; }

        public Dictionary<string, int> ServiceWeights { get; set; }

        public Dictionary<string, object> ServiceMeta { get; set; }

        public int ServicePort { get; set; }

        public bool ServiceEnableTagOverride { get; set; }

        public string ServiceProxyDestination { get; set; }

        public Dictionary<string, object> ServiceProxy { get; set; }

        public Dictionary<string, object> ServiceConnect { get; set; }

        public int CreateIndex { get; set; }

        public int ModifyIndex { get; set; }
    }

    public class TaggedAddresses
    {
        public string lan { get; set; }
        public string wan { get; set; }
    }
}