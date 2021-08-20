
using Service.FileService;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Service.BusinessModels.CloudEnvironment;
using AutoMapper;
using Service.BusinessModels;
using Service.VulnerabilityService;
using Contracts.CloudEnvironment;
using Service.StatsService;
using Contracts.BusinessModels.CloudEnvironment;
using System.Diagnostics;

namespace Service.CloudEnvironment
{
    public class CloudEnvironmentService : ICloudEnvironmentService
    {
        private readonly IFileService _fileService;
        private readonly IVulnerabilityService _vulnerabilityService;
        private readonly IStatsService _statsService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private Stopwatch _stopwatch;


        public CloudEnvironmentService(IFileService fileService,
            IVulnerabilityService vulnerabilityService,
            IStatsService statsService,
            IMapper mapper, 
            ILogger<CloudEnvironmentService> logger = null)
        {
            _fileService = fileService;
            _vulnerabilityService = vulnerabilityService;
            _statsService = statsService;
            _mapper = mapper;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task<CloudEnvironmentResponseEntity> Attack(CloudEnvironmentRequestEntity cloudEnvironmentEntity)
        {
            _logger.LogInformation($"Start -> Attack, perform attack on: {cloudEnvironmentEntity.vmName}.");
            var vulnerabilityRequestEntity = _mapper.Map<VulnerabilityRequestEntity>(cloudEnvironmentEntity);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            vulnerabilityRequestEntity.CloudEnvironment = await GetCloudEnvironment().ConfigureAwait(false);
            var result = _vulnerabilityService.GetPotentialAttackersVms(vulnerabilityRequestEntity);

            stopwatch.Stop();
            await _statsService.SaveStat(new StatsRequestEntity 
            { 
                NumberOfVirtualMachines = vulnerabilityRequestEntity.CloudEnvironment.vms.Count, 
                RequestTime = stopwatch.ElapsedMilliseconds
            }).ConfigureAwait(false);
            _stopwatch.Reset();
            var response = _mapper.Map<CloudEnvironmentResponseEntity>(result);
            _logger.LogInformation($"Finished -> Attack, perform attack on: {cloudEnvironmentEntity.vmName}.");
            return response;
        }

        public async Task<StatsResponseEntity> GetStats()
        {
            _stopwatch.Start();
            var res = await _statsService.GetStat().ConfigureAwait(false);
            _stopwatch.Stop();
            await _statsService.SaveStat(new StatsRequestEntity
            {
                NumberOfVirtualMachines = res?.NumberOfVirtualMachines,
                RequestTime = _stopwatch.ElapsedMilliseconds
            }).ConfigureAwait(false);
            _stopwatch.Reset();
            return res;
        }

        private async Task<Contracts.CloudEnvironment.CloudEnvironment> GetCloudEnvironment()
        {
            var inputFilePath = Environment.GetCommandLineArgs();
            var cloudEnvironment = await _fileService.LoadJsonFileDataAsync(inputFilePath[1]).ConfigureAwait(false);
            return cloudEnvironment;
        }
    }
}
