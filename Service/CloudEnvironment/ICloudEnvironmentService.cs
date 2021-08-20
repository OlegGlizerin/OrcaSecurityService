using Contracts.CloudEnvironment;
using Service.BusinessModels.CloudEnvironment;
using System.Threading.Tasks;

namespace Service.CloudEnvironment
{
    public interface ICloudEnvironmentService
    {
        Task<StatsResponseEntity> GetStats();
        Task<CloudEnvironmentResponseEntity> Attack(CloudEnvironmentRequestEntity cloudEnvironmentEntity);
    }
}
