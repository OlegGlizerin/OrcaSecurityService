
using AutoMapper;
using Contracts.BusinessModels.CloudEnvironment;
using Contracts.CloudEnvironment;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.BusinessModels;
using Service.BusinessModels.CloudEnvironment;
using Service.BusinessModels.Vulnerability;
using Service.CloudEnvironment;
using Service.FileService;
using Service.StatsService;
using Service.VulnerabilityService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    [TestClass]
    public class CloudEnvironmentServiceTests
    {
        private ICloudEnvironmentService _cloudEnvironmentService;
        private IFileService _fileService;
        private IVulnerabilityService _vulnerabilityService;
        private IStatsService _statsService;
        private IMapper _mapper;
        private ILogger<CloudEnvironmentService> _logger;

        private VulnerabilityRequestEntity _vulnerabilityRequestEntity;
        private VulnerabilityRequestEntity _vulnerabilityRequestEntityNonHappy;
        private VulnerabilityResponseEntity _vulnerabilityResponseEntity;
        private VulnerabilityResponseEntity _vulnerabilityResponseEntityNonHappy;
        private CloudEnvironmentResponseEntity _cloudEnvironmentResponseEntity;
        private CloudEnvironment _cloudEnvironment;
        private List<string> _potentialAttackers;
        private string _vmToAtack = "batman";

        [TestInitialize]
        public void TestInit()
        {
            _fileService = A.Fake<IFileService>();
            _vulnerabilityService = A.Fake<IVulnerabilityService>();
            _statsService = A.Fake<IStatsService>();
            _mapper = A.Fake<IMapper>();
            _logger = A.Fake<ILogger<CloudEnvironmentService>>();

            _cloudEnvironment = new CloudEnvironment
            {
                fw_rules = new List<FwRule>() { new FwRule { dest_tag = "qwe", fw_id = "12", source_tag = "qw" } },
                vms = new List<Vm>() { new Vm { name = "qwe", tags = new List<string>() { "qwe1", "asd1" }, vm_id = "18347" } }
            };

            _vulnerabilityRequestEntity = new VulnerabilityRequestEntity()
            {
                CloudEnvironment = _cloudEnvironment,
                vmName = _vmToAtack
            };

            _vulnerabilityRequestEntityNonHappy = new VulnerabilityRequestEntity();

            _potentialAttackers = new List<string>() { "attacker1", "attacker2" };

            _vulnerabilityResponseEntity = new VulnerabilityResponseEntity()
            {
                PotentialAttackers = _potentialAttackers
            };

            _vulnerabilityResponseEntityNonHappy = new VulnerabilityResponseEntity();

            _cloudEnvironmentResponseEntity = new CloudEnvironmentResponseEntity
            {
                PotentialAttackers = _potentialAttackers
            };


            _cloudEnvironmentService = new CloudEnvironmentService(_fileService, _vulnerabilityService, _statsService,
                _mapper, _logger);
        }

        [TestMethod]
        public async Task Attack_HappyFlow_PotentialAttackersAreAsExpected()
        {
            //Arrange
            ArrangeData();

            //Act
            var res = await _cloudEnvironmentService.Attack(new CloudEnvironmentRequestEntity 
            { 
                vmName = _vmToAtack
            });

            //Assert
            CollectionAssert.AreEqual(_potentialAttackers, res.PotentialAttackers.ToList());
        }

        [TestMethod]
        public async Task Attack_AttackerVmNotFound_PotentialAttackersAreAsExpected()
        {
            //Arrange
            ArrangeData(happyFlow: false);

            //Act
            var res = await _cloudEnvironmentService.Attack(new CloudEnvironmentRequestEntity
            {
                vmName = _vmToAtack
            });

            //Assert
            Assert.AreEqual(_vulnerabilityResponseEntityNonHappy.PotentialAttackers, res);
        }

        private void ArrangeData(bool happyFlow = true)
        {
            A.CallTo(() => _mapper.Map<VulnerabilityRequestEntity>(A<CloudEnvironmentRequestEntity>._))
                .Returns(_vulnerabilityRequestEntity);

            A.CallTo(() => _fileService.LoadJsonFileDataAsync(A<string>._))
                .Returns(_cloudEnvironment);

            var vulnerabilityReq = happyFlow == true ? _vulnerabilityRequestEntity : _vulnerabilityRequestEntityNonHappy;
            var vulnerabilityRes = happyFlow == true ? _vulnerabilityResponseEntity : _vulnerabilityResponseEntityNonHappy;

            A.CallTo(() => _vulnerabilityService.GetPotentialAttackersVms(vulnerabilityReq))
                .Returns(vulnerabilityRes);

            A.CallTo(() => _statsService.SaveStat(A<StatsRequestEntity>._))
                .Returns(true);

            var mapRes = happyFlow == true ? _cloudEnvironmentResponseEntity : null;
            A.CallTo(() => _mapper.Map<CloudEnvironmentResponseEntity>(A<VulnerabilityResponseEntity>._))
                .Returns(mapRes);
        }
    }
}
