using Contracts.BusinessModels.CloudEnvironment;
using Contracts.CloudEnvironment;
using System.Threading.Tasks;

namespace Service.StatsService
{
    public interface IStatsService
    {
        public Task<StatsResponseEntity> GetStat();
        public Task<bool> SaveStat(StatsRequestEntity statRequestEntity);
    }
}
