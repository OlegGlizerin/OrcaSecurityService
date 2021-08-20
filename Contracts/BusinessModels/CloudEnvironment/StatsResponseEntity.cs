namespace Contracts.CloudEnvironment
{
    public class StatsResponseEntity
    {
        public int? NumberOfVirtualMachines { get; set; }
        public int NumberOfRequests { get; set; }
        public float AverrageRequestProcessingTime { get; set; }
    }
}
