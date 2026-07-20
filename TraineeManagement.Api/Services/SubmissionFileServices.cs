using System.Security.Cryptography;
using System.Web;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.SubmissionFilesDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
// using TraineeManagement.Shared.Models;
using TraineeManagement.Api.Settings;

using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Codeuctivity;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Utils;

namespace TraineeManagement.Api.Services
{
    public class SubmissionFileServices : ISubmissionFileService
    {

        private readonly AppDbContext _context;
        private readonly IFileStorageService _service;
        private readonly ISubmissionProcessingPublisherService _publisherService;
        private readonly FileStorageSettings _settings;
        private readonly ILogger<SubmissionFileServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubmissionFileServices(
            AppDbContext context,
            IFileStorageService service,
            ISubmissionProcessingPublisherService publisherService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SubmissionFileServices> logger,
            IOptions<FileStorageSettings> options
        )
        {
            _context = context;
            _service = service;
            _publisherService = publisherService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _settings = options.Value;
        }


        public static string CleanAndSanitize(string input)
        {
            string safe = SanitizeFilename.Sanitize(input);

            safe = Regex.Replace(safe, @"-+", "-");

            safe = safe.TrimEnd('.');

            safe = Regex.Replace(safe, @"\.+", ".");

            return safe;
        }

        public async Task<SubmissionFileResponse> GetSubmissionFileById(int id)
        {
            var metadata = await _context.SubmissionFiles.FindAsync(id) ??
                throw new KeyNotFoundException($"Submission file metadata not found for file with Id {id}");

            return new SubmissionFileResponse()
            {
                Id = metadata.Id,
                TraceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Context",
                OriginalFileName = metadata.OriginalFileName,
                ContentType = metadata.ContentType,
                FileSize = metadata.FileSize,
                CreatedAt = metadata.CreatedAt
            };
        }

        public async Task<SubmissionFileResponse> UploadFile(int submissionId, IFormFile formFile, int userId)
        {
            long size = formFile.Length;

            if (formFile == null || size == 0)
                throw new ArgumentException("File must not be empty");

            if (size > _settings.MaxFileSizeBytes)
            {
                var maxSizeMb = _settings.MaxFileSizeBytes / (1024 * 1024);
                throw new FileTooLargeException($"File size greater than {maxSizeMb} MB.");
            }

            var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtentions.Contains(extension))
                throw new ArgumentException("File extension is not supported");

            if (!_settings.AllowedContentTypes.Contains(formFile.ContentType))
                throw new ArgumentException("File Content Type is not supported");

            var submission = await _context.Submissions.FindAsync(submissionId) ??
                throw new KeyNotFoundException("Submission not found");

            var user = await _context.Users.FindAsync(userId) ??
                throw new KeyNotFoundException("User not found");

            string checksum;
            using (var sha256 = SHA256.Create())
            {
                using var checksumStream = formFile.OpenReadStream();
                checksum = Convert.ToHexString(await sha256.ComputeHashAsync(checksumStream));
            }

            var GeneratedFileName = await _service.SaveAsync(formFile.OpenReadStream(), extension);
            SubmissionFile metadata = new SubmissionFile()
            {
                OriginalFileName = CleanAndSanitize(formFile.FileName),
                GeneratedFileName = GeneratedFileName,
                ContentType = formFile.ContentType,
                FileSize = size,
                CheckSum = checksum,
                SubmissionId = submissionId,
                UserId = userId
            };

            await _context.SubmissionFiles.AddAsync(metadata);
            await _context.SaveChangesAsync();

            var savedMetadata = await _context.SubmissionFiles.FindAsync(metadata.Id) ??
                throw new Exception($"Error while saving metadata for file {metadata.Id}");

            _logger.LogInformation("File Uploaded successfully with the following metadata: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}, CreatedDate: {CreatedDate}", metadata.Id, metadata.OriginalFileName, metadata.FileSize, metadata.ContentType, metadata.CreatedAt);

            // adding RabbitMQ message queue
            var message = new SubmissionProcessingRequested
            {
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Context",
                SubmissionId = submissionId,
                FileId = metadata.Id
            };

            try
            {
                await _publisherService.PublishAsync(message);
            }
            catch (Exception ex)
            {
                _context.SubmissionFiles.Remove(metadata);
                await _service.DeleteAsync(metadata.GeneratedFileName);
                await _context.SaveChangesAsync();

                throw new Exception($"Connection error encountered from producer side: {ex.Message}");
            }

            var job = new ProcessingJob()
            {
                MessageId = message.MessageId,
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Context",
                SubmissionId = submissionId,
                FileId = metadata.Id
            };

            await _context.ProcessingJobs.AddAsync(job);
            await _context.SaveChangesAsync();

            var savedJob = await _context.ProcessingJobs.FindAsync(job.Id) ??
                throw new Exception($"Error while saving job with Id: {job.Id}");

            _logger.LogInformation("Processing of job {status} with Id: {jobId} added", job.Status, job.Id);

            return new SubmissionFileResponse()
            {
                Id = metadata.Id,
                TraceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "No Context",
                OriginalFileName = metadata.OriginalFileName,
                ContentType = metadata.ContentType,
                FileSize = metadata.FileSize,
                CreatedAt = metadata.CreatedAt
            };
        }

        public async Task<FileDownloadResponse> DownloadFile(int id, ClaimsPrincipal User)
        {
            int Id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var submissionFile = await _context.SubmissionFiles
                                .Include(x => x.User)
                                .FirstOrDefaultAsync(x => x.Id == id) ??

                throw new KeyNotFoundException("File Resource not found");

            if (submissionFile.UserId != Id || !AccessDefination.AllowedRolesToDownload.Contains(role))
            {
                throw new AccessForbiddenException("Access Denied to download the file");
            }

            var exists = await _service.ExistsAsync(submissionFile.GeneratedFileName);
            if (!exists)
            {
                throw new FileNotFoundException("Physical file not found");
            }

            Stream fileStream = await _service.OpenReadAsync(submissionFile.GeneratedFileName);

            return new FileDownloadResponse()
            {
                Stream = fileStream,
                ContentType = submissionFile.ContentType,
                FileName = submissionFile.OriginalFileName
            };
        }

        public async Task DeleteFile(int id)
        {
            var submissionFile = await _context.SubmissionFiles.FindAsync(id) ??
                throw new KeyNotFoundException($"File metadat for resource with Id {id} not found");

            await _service.DeleteAsync(submissionFile.GeneratedFileName);
            _context.SubmissionFiles.Remove(submissionFile);

            await _context.SaveChangesAsync();
            _logger.LogCritical("Submission File deleted with Id {id}", id);
        }
    }
}