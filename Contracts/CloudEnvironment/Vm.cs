using System.Collections.Generic;

namespace Contracts.CloudEnvironment
{
    public class Vm
    {
        public string vm_id { get; set; }
        public string name { get; set; }
        public List<string> tags { get; set; }
    }
}
