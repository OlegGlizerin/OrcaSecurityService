using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.CloudEnvironment
{
    public class FwRule
    {
        public string fw_id { get; set; }
        public string source_tag { get; set; }
        public string dest_tag { get; set; }
    }
}
