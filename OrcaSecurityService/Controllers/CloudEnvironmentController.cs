using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Service.BusinessModels.CloudEnvironment;
using Newtonsoft.Json;
using System.Linq;
using Service.CloudEnvironment;

namespace OrcaSecurityService.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CloudEnvironmentController : ControllerBase
    {

        private readonly ICloudEnvironmentService _cloudEnvironmentService;

        public CloudEnvironmentController(ICloudEnvironmentService cloudEnvironmentService)
        {
            _cloudEnvironmentService = cloudEnvironmentService;
        }

        [HttpGet]
        [Route("/stats")]
        public async Task<string> Stats()
        {
            var res = await _cloudEnvironmentService.GetStats().ConfigureAwait(false);
            string jsonRes = JsonConvert.SerializeObject(res);
            return jsonRes;
        }

        [HttpGet]
        [Route("/attack/{vm_id}")]
        public async Task<string> Attack(string vm_id)
        {
            var cloudEnvironmentEntity = new CloudEnvironmentRequestEntity { vmName = vm_id };
            var res = await _cloudEnvironmentService.Attack(cloudEnvironmentEntity).ConfigureAwait(false);
            string jsonRes = JsonConvert.SerializeObject(res.PotentialAttackers.Select(x => x));
            return jsonRes;
        }
    }
}
