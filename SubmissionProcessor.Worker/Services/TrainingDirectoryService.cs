// using Microsoft.EntityFrameworkCore;
// using SubmissionProcessor.Worker.Interfaces;
// using Microsoft.Extensions.Options;
// using SubmissionProcessor.Worker.Settings;
// using SubmissionProcessor.Worker.DTOs.TraineeProfileDto;
// using System.Net;
// using System.Net.Http.Json;

// namespace SubmissionProcessor.Worker.Services;

// public class TrainingDirectoryService
// {
//     private readonly ITrainingDirectoryClient _client;
//     private readonly ILogger<TrainingDirectoryService> _logger;

//     public TrainingDirectoryService(
//         ITrainingDirectoryClient client,
//         ILogger<TrainingDirectoryService> logger
//      )
//     {
//         _client = client;
//         _logger = logger;
//     }
//     public async Task<TraineeProfileResponse?> GetTraineeProfileWithFallbackAsync(int traineeId, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var response = await _client.GetProfiles(traineeId, cancellationToken);
//             if (response == null)
//             {
//                 return new TraineeProfileResponse
//                 {
//                     Id = traineeId,
//                     Name = "Unknown",
//                     Email = "unknown@gmail.com",
//                     Designation = "SE"
//                 };
//             }
//             return response;
//         }
//         catch (Exception e)
//         {
//             _logger.LogError(e, "Training Directory Unavailable for trainee profile {TraineeId}", traineeId);
//             return new TraineeProfileResponse
//             {
//                 Id = traineeId,
//                 Name = "Fallback Profile",
//                 Email = "",
//                 Designation = "Fallback Designation"
//             };
//         }
//     }
// }