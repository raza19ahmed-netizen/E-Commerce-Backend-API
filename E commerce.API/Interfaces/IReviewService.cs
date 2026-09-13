using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateAsync(
        int userId,
        int productId,
        CreateReviewDto dto);

    Task<IEnumerable<ReviewDto>> GetByProductIdAsync(int productId);

    Task<ProductRatingDto> GetRatingSummaryAsync(int productId);

    Task<ReviewDto> UpdateAsync(
    int reviewId,
    int userId,
    UpdateReviewDto dto);

    Task DeleteAsync(int reviewId, int userId);
}