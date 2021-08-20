using AutoMapper;
using Service.BusinessModels;
using Service.BusinessModels.CloudEnvironment;
using Service.BusinessModels.Vulnerability;

namespace OrcaSecurityService.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CloudEnvironmentRequestEntity, VulnerabilityRequestEntity>()
                .ForMember(dest => dest.vmName, opt => opt.MapFrom(src => src.vmName));
            CreateMap<VulnerabilityResponseEntity, CloudEnvironmentResponseEntity>();
        }
    }
}
