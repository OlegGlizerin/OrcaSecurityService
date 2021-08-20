using Contracts.BusinessModels.CloudEnvironment;
using Contracts.CloudEnvironment;
using Microsoft.Extensions.Logging;
using Service.FileService;
using System.Threading.Tasks;

namespace Service.StatsService
{
    public class StatsService : IStatsService
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;

        public StatsService(IFileService fileService, ILogger<StatsService> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<bool> SaveStat(StatsRequestEntity statRequestEntity)
        {
            _logger.LogInformation($"Start -> SaveStat, saving statistics, NumberOfVirtualMachines: {statRequestEntity?.NumberOfVirtualMachines}, RequestProcessingTime: {statRequestEntity.RequestTime}");
            var successfullSave = false;
            var statsResponseEntity = await _fileService.GetStat().ConfigureAwait(false);
            if (statsResponseEntity == null)
            {
                var statsEntityNew = new StatsResponseEntity
                {
                    NumberOfVirtualMachines = statRequestEntity.NumberOfVirtualMachines,
                    AverrageRequestProcessingTime = statRequestEntity.RequestTime,
                    NumberOfRequests = 1
                };
                await _fileService.SaveStat(statsEntityNew).ConfigureAwait(false);
                _logger.LogInformation($"Finished -> SaveStat, saving statistics for the first time, NumberOfVirtualMachines: {statsEntityNew?.NumberOfVirtualMachines}, NumberOfRequests: {statsEntityNew.NumberOfRequests}, AverrageRequestProcessingTime: {statsEntityNew.AverrageRequestProcessingTime}");
            }
            else
            {
                var averageRequestsTimeOld = statsResponseEntity.AverrageRequestProcessingTime;
                var currentRequestTime = statRequestEntity.RequestTime;
                var numOfRequests = statsResponseEntity.NumberOfRequests + 1;
                float newAverageRequestTime = averageRequestsTimeOld + (currentRequestTime - averageRequestsTimeOld) / numOfRequests;

                var statsResponseEntityNew = new StatsResponseEntity
                {
                    NumberOfVirtualMachines = statRequestEntity.NumberOfVirtualMachines,
                    AverrageRequestProcessingTime = newAverageRequestTime,
                    NumberOfRequests = statsResponseEntity.NumberOfRequests + 1
                };
                await _fileService.SaveStat(statsResponseEntityNew).ConfigureAwait(false);
                _logger.LogInformation($"Finished -> SaveStat, saving statistics, NumberOfVirtualMachines: {statsResponseEntity?.NumberOfVirtualMachines}, NumberOfRequests: {statsResponseEntity.NumberOfRequests}, AverrageRequestProcessingTime: {statsResponseEntityNew.AverrageRequestProcessingTime}");
            }
            return successfullSave;
        }

        public async Task<StatsResponseEntity> GetStat()
        {
            var statsResponseEntity = await _fileService.GetStat().ConfigureAwait(false);
            return statsResponseEntity;
        }
    }
}
