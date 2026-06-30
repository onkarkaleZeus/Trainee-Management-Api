using Microsoft.Extensions.Options;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Settings;

namespace TraineeManagement.Api.Services
{
    public class LocalFileStorageServices : IFileStorageService
    {

        private readonly string _rootPath;
        private readonly ILogger<LocalFileStorageServices> _logger;

        public LocalFileStorageServices(ILogger<LocalFileStorageServices> logger, IOptions<FileStorageSettings> options)
        {
            _rootPath = options.Value.RootPath;
            Directory.CreateDirectory(_rootPath);

            _logger = logger;
        }
        public async Task<string> SaveAsync(Stream stream, string extension)
        {
            var GeneratedFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_rootPath, GeneratedFileName);
            using var fileStream = File.Create(fullPath);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation("File saved on physical storage");
            return GeneratedFileName;
        }

        public Task<Stream> OpenReadAsync(string GeneratedFileName)
        {
            var fullPath = Path.Combine(_rootPath, GeneratedFileName);
            Stream stream = File.OpenRead(fullPath);

            return Task.FromResult(stream);
        }

        public Task<bool> ExistsAsync(string GeneratedFileName)
        {
            var fullPath = Path.Combine(_rootPath, GeneratedFileName);
            if (File.Exists(fullPath))
            {
                _logger.LogInformation("File exists on the physical storage");
                return Task.FromResult(true);
            }

            _logger.LogCritical("File doesn't exist on the physical storage");
            return Task.FromResult(false);
        }

        public Task DeleteAsync(string GeneratedFileName)
        {
            var fullPath = Path.Combine(_rootPath, GeneratedFileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found on Physical storage");
            }

            File.Delete(fullPath);
            _logger.LogInformation("File removed from Physical storage");
            return Task.CompletedTask;
        }
    }
}