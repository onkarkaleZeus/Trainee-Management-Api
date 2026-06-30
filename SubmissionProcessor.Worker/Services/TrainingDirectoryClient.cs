using System.Net;
using System.Net.Http.Json;
using SubmissionProcessor.Worker.DTOs;
using SubmissionProcessor.Worker.DTOs.TraineeProfileDto;
using SubmissionProcessor.Worker.Interfaces;
 
namespace SubmissionProcessor.Worker.Services;
 
public class TrainingDirectoryClient : ITrainingDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TrainingDirectoryClient> _logger;
 
    public TrainingDirectoryClient(
        HttpClient httpClient,
        ILogger<TrainingDirectoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
 
    public async Task<TraineeProfileResponse?> GetProfiles(
        int traineeId,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"TraineeProfile/{traineeId}");
 
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }
 
        var response = await _httpClient.SendAsync(request, cancellationToken);
 
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                "Trainee profile not found in TrainingDirectory for TraineeId {TraineeId}",
                traineeId);
 
            return null;
        }
 
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
 
            _logger.LogError(
                "TrainingDirectory trainee lookup failed for TraineeId {TraineeId}. StatusCode: {StatusCode}",
                traineeId,
                (int)response.StatusCode);
 
            throw new HttpRequestException(
                $"TrainingDirectory trainee lookup failed. StatusCode={(int)response.StatusCode}. Response={error}");
        }
 
        return await response.Content.ReadFromJsonAsync<TraineeProfileResponse>(
            cancellationToken: cancellationToken);
    }
}
 