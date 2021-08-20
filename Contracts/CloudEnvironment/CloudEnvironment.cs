using System.Collections.Generic;

namespace Contracts.CloudEnvironment
{
    public class CloudEnvironment
    {
        public List<Vm> vms { get; set; }
        public List<FwRule> fw_rules { get; set; }
    }
}
