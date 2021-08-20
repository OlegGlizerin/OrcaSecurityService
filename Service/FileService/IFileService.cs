using Contracts.CloudEnvironment;
using System.Threading.Tasks;

namespace Service.FileService
{
    public interface IFileService
    {
        Task<Contracts.CloudEnvironment.CloudEnvironment> LoadJsonFileDataAsync(string pathToInputFile);
        Task<StatsResponseEntity> GetStat();
        Task<bool> SaveStat(StatsResponseEntity statRequestEntity);
    }
}
