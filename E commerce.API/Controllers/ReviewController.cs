using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [Authorize]
    [HttpPost("product/{productId}")]
    public async Task<IActionResult> CreateReview(
    int productId,
    CreateReviewDto dto)
    {
        // JWT Token से logged-in user की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var review = await _reviewService
            .CreateAsync(userId, productId, dto);

        return Ok(new ApiResponse<ReviewDto>(
            true,
            "Review created successfully.",
            review));
    }
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProductReviews(int productId)
    {
        var reviews = await _reviewService
            .GetByProductIdAsync(productId);

        return Ok(new ApiResponse<IEnumerable<ReviewDto>>(
            true,
            "Reviews retrieved successfully.",
            reviews));
    }

    [HttpGet("product/{productId}/rating-summary")]
    public async Task<IActionResult> GetRatingSummary(int productId)
    {
        var ratingSummary = await _reviewService
            .GetRatingSummaryAsync(productId);

        return Ok(new ApiResponse<ProductRatingDto>(
            true,
            "Product rating summary retrieved successfully.",
            ratingSummary));
    }

    [Authorize]
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReview(
    int reviewId,
    UpdateReviewDto dto)
    {
        // JWT Token से logged-in user की ID लो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var updatedReview = await _reviewService
            .UpdateAsync(reviewId, userId, dto);

        return Ok(new ApiResponse<ReviewDto>(
            true,
            "Review updated successfully.",
            updatedReview));
    }

    [Authorize]
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        // JWT से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _reviewService.DeleteAsync(reviewId, userId);

        return Ok(new ApiResponse<object>(
            true,
            "Review deleted successfully.",
            null));
    }
}