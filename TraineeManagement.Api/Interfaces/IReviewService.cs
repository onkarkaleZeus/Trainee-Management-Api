using TraineeManagement.Api.DTOs.ReviewDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetAllReviews(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // ReviewStatus? status
        );
        Task<ReviewResponse?> GetReviewById(int id);
        Task<ReviewResponse> AddReview(CreateReviewRequest request);
    }
}