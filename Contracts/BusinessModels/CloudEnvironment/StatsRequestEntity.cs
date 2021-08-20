using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.BusinessModels.CloudEnvironment
{
    public class StatsRequestEntity
    {
        public int? NumberOfVirtualMachines { get; set; }
        public float RequestTime { get; set; }
    }
}
