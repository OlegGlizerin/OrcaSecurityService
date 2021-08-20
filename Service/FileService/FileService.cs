
using Contracts.CloudEnvironment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Service.FileService
{
    public class FileService : IFileService
    {
        private readonly ILogger _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        private string _pathToStatFile = AppDomain.CurrentDomain.BaseDirectory + "stat.txt";


        public async Task<Contracts.CloudEnvironment.CloudEnvironment> LoadJsonFileDataAsync(string pathToInputFile)
        {
            Contracts.CloudEnvironment.CloudEnvironment cloudEnvironment = new Contracts.CloudEnvironment.CloudEnvironment();
            try
            {
                _logger.LogInformation($"Start -> LoadJsonFilesData, trying reading the file in: {pathToInputFile}.");
                char[] result;
                StringBuilder builder = new StringBuilder();
                using (StreamReader reader = File.OpenText(pathToInputFile))
                {
                    result = new char[reader.BaseStream.Length];
                    await reader.ReadAsync(result, 0, (int)reader.BaseStream.Length);
                }

                foreach (char c in result)
                {
                    builder.Append(c);
                }

                cloudEnvironment = JsonConvert.DeserializeObject<Contracts.CloudEnvironment.CloudEnvironment>(builder.ToString());
                _logger.LogInformation($"Finish -> LoadJsonFilesData, Finished reading the file, found vms: {cloudEnvironment.vms.Count} and fw_rules: {cloudEnvironment.fw_rules.Count}, in: {pathToInputFile}.");
                return cloudEnvironment;
            }
            catch(Exception e)
            {
                _logger.LogInformation($"Error -> LoadJsonFilesData, ex: {e}");
            }
            return cloudEnvironment;
        }

        public async Task<StatsResponseEntity> GetStat()
        {
            StatsResponseEntity result = null;
            try
            {
                var jsonData = await File.ReadAllTextAsync(_pathToStatFile, Encoding.UTF8).ConfigureAwait(false);
                result = JsonConvert.DeserializeObject<StatsResponseEntity>(jsonData);
            }
            catch (FileNotFoundException e)
            {
                _logger.LogWarning($"Finished -> GetStat, stat.txt file not found, lets create a new one.");
                InitStatFile(); //create stat.txt file
            }
            catch (Exception e)
            {
                _logger.LogError($"Finished -> GetStat, Error happened, ex: {e}");
            }
           
            return result;
        }

        public async Task<bool> SaveStat(StatsResponseEntity statsResponseEntity)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(statsResponseEntity);
                await File.WriteAllTextAsync(_pathToStatFile, jsonData).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError($"Finished -> SaveStat, Error happened, ex: {e}");
                return false;
            }
            return true;
        }

        private void InitStatFile()
        {
            _logger.LogInformation($"Start -> InitStatFile, crating empty stat file.");
            File.WriteAllText(_pathToStatFile, string.Empty);
            _logger.LogInformation($"Finish -> InitStatFile, crating empty stat file.");
        }
    }
}
