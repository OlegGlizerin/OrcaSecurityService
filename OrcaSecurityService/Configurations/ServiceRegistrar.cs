using Microsoft.Extensions.DependencyInjection;
using Service.CloudEnvironment;
using Service.FileService;
using Service.StatsService;
using Service.VulnerabilityService;

namespace OrcaSecurityService.Configurations
{
    public class ServiceRegistrar
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ICloudEnvironmentService, CloudEnvironmentService>();
            services.AddSingleton<IVulnerabilityService, VulnerabilityService>();
            services.AddSingleton<IStatsService, StatsService>();
        }
    }
}
