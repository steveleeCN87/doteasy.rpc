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

/*
[
    {
        "ID": "79428e1c-858b-f140-f229-daeb5fc88bbc",
        "Node": "consul-1",
        "Address": "192.168.153.129",
        "Datacenter": "dc1",
        "TaggedAddresses": {
            "lan": "192.168.153.129",
            "wan": "192.168.153.129"
        },
        "NodeMeta": {
            "consul-network-segment": ""
        },
        "ServiceKind": "",
        "ServiceID": "7a492116-df0e-4d2d-adc9-254056c0df49",
        "ServiceName": "Rpc.Common.IUserService.GetUserId_userName",
        "ServiceTags": [
            "urlprefix-/Rpc.Common.IUserService.GetUserId_userName"
        ],
        "ServiceAddress": "192.168.7.54",
        "ServiceWeights": {
            "Passing": 1,
            "Warning": 1
        },
        "ServiceMeta": {},
        "ServicePort": 9881,
        "ServiceEnableTagOverride": false,
        "ServiceProxyDestination": "",
        "ServiceProxy": {},
        "ServiceConnect": {},
        "CreateIndex": 173,
        "ModifyIndex": 173
    }
]

*/