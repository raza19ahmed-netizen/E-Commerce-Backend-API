using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }

    public async Task<ReviewDto> CreateAsync(
        int userId,
        int productId,
        CreateReviewDto dto)
    {
        // पहले check करो कि Product मौजूद है या नहीं
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Check करो कि User ने पहले से इसी Product को review किया है या नहीं
        var alreadyReviewed = await _reviewRepository
            .ExistsAsync(userId, productId);

        if (alreadyReviewed)
        {
            throw new InvalidOperationException(
                "You have already reviewed this product.");
        }

        // नया Review object बनाओ
        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        // Database में save करो
        var savedReview = await _reviewRepository.AddAsync(review);

        // DTO response return करो
        return new ReviewDto
        {
            Id = savedReview.Id,
            Rating = savedReview.Rating,
            Comment = savedReview.Comment,
            CreatedAt = savedReview.CreatedAt,

            // अभी हमें logged-in user का Name चाहिए
            // अगले step में इसे properly handle करेंगे
            UserName = string.Empty
        };
    }

    public async Task<IEnumerable<ReviewDto>> GetByProductIdAsync(int productId)
    {
        // पहले check करो कि Product मौजूद है या नहीं
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Product के सभी Reviews प्राप्त करो
        var reviews = await _reviewRepository
            .GetByProductIdAsync(productId);

        // Reviews को DTO में convert करो
        return reviews.Select(review => new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UserName = review.User.Name
        }).ToList();
    }
    public async Task<ProductRatingDto> GetRatingSummaryAsync(int productId)
    {
        // पहले check करो कि Product मौजूद है या नहीं
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Repository से Average Rating और Total Reviews प्राप्त करो
        var (averageRating, totalReviews) =
            await _reviewRepository.GetRatingSummaryAsync(productId);

        // Clean DTO response return करो
        return new ProductRatingDto
        {
            ProductId = productId,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = totalReviews
        };
    }

    public async Task<ReviewDto> UpdateAsync(
    int reviewId,
    int userId,
    UpdateReviewDto dto)
    {
        // सिर्फ उसी user का Review प्राप्त करो
        var review = await _reviewRepository
            .GetByIdAndUserIdAsync(reviewId, userId);

        if (review == null)
        {
            throw new KeyNotFoundException(
                "Review not found or you do not have permission to update it.");
        }

        // केवल Rating और Comment update होंगे
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        // Database में changes save करो
        await _reviewRepository.UpdateAsync(review);

        // Updated Review का clean response वापस भेजो
        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UserName = review.User.Name
        };
    }

    public async Task DeleteAsync(int reviewId, int userId)
    {
        // सिर्फ उसी logged-in user का Review प्राप्त करो
        var review = await _reviewRepository
            .GetByIdAndUserIdAsync(reviewId, userId);

        if (review == null)
        {
            throw new KeyNotFoundException(
                "Review not found or you do not have permission to delete it.");
        }

        // Database से Review delete करो
        await _reviewRepository.DeleteAsync(review);
    }
}