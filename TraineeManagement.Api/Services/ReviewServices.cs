using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.SubmissionDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.VisualBasic;
using TraineeManagement.Api.DTOs.ReviewDto;

namespace TraineeManagement.Api.Services
{
    public class ReviewServices(
        ILogger<ReviewServices> logger,
        AppDbContext context

    ) : IReviewService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<ReviewServices> _logger = logger;

        public async Task<IEnumerable<ReviewResponse>> GetAllReviews(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // TaskAssignmentStatus? status
        )
        {
            var records = await _context.Reviews
                                        .Include(x => x.Submission)
                                        .Include(x => x.Mentor)
                                        .ToListAsync();

            _logger.LogInformation("Review records retrieved : {recordCount}", records.Count);
            return records.Select(MapDtoToReviewResponse);

        }

        public async Task<ReviewResponse?> GetReviewById(int id)
        {
            var review = await _context.Reviews
                                        .Include(x => x.Submission)
                                        .Include(x => x.Mentor)
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review not found for Id: {id}");
            }

            _logger.LogInformation("Review with Id {id} found", id);
            return MapDtoToReviewResponse(review);

        }

        public async Task<ReviewResponse> AddReview(CreateReviewRequest request)
        {

            var taskAssignment = await _context.Submissions.FindAsync(request.SubmissionId) ??
                throw new KeyNotFoundException("Submission Id is invalid");

            var mentor = await _context.Mentors.FindAsync(request.MentorId) ??
                throw new KeyNotFoundException("Mentor Id is invalid");

            Review newReview = new Review()
            {
                SubmissionId = request.SubmissionId,
                MentorId = request.MentorId,
                Feedback = request.Feedback,
                Score = request.Score,
                ReviewStatus = request.Status,
                ReviewedDate = request.ReviewedDate,
            };

            await _context.Reviews.AddAsync(newReview);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Review added with Id {Id}", newReview.Id);
            return MapDtoToReviewResponse(newReview);
        }

        public ReviewResponse MapDtoToReviewResponse(Review review)
        {
            return new ReviewResponse()
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                SubmissionUrl = review.Submission.SubmissionUrl,
                SubmissionDate = review.Submission.SubmittedDate,
                MentorId = review.MentorId,
                MentorName = $"{review.Mentor.FirstName} {review.Mentor.LastName}",
                Feedback = review.Feedback,
                Score = review.Score,
                Status = review.ReviewStatus,
                ReviewedDate = review.ReviewedDate,
                CreatedDate = review.CreatedAt,
                UpdatedDate = review.UpdatedAt
            };
        }

    }
}