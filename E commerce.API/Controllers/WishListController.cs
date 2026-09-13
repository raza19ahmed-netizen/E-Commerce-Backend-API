using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpPost("product/{productId}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        // JWT Token से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Product को logged-in user की Wishlist में add करो
        var wishlistItem = await _wishlistService
            .AddAsync(userId, productId);

        return Ok(new ApiResponse<WishlistItemDto>(
            true,
            "Product added to wishlist successfully.",
            wishlistItem));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist()
    {
        // JWT Token से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // सिर्फ उसी logged-in user की Wishlist प्राप्त करो
        var wishlist = await _wishlistService
            .GetMyWishlistAsync(userId);

        return Ok(new ApiResponse<IEnumerable<WishlistItemDto>>(
            true,
            "Wishlist retrieved successfully.",
            wishlist));
    }

    [HttpDelete("{wishlistItemId}")]
    public async Task<IActionResult> RemoveFromWishlist(int wishlistItemId)
    {
        // JWT Token से logged-in User की ID प्राप्त करो
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // सिर्फ उसी user का Wishlist Item remove होगा
        await _wishlistService.RemoveAsync(wishlistItemId, userId);

        return Ok(new ApiResponse<object>(
            true,
            "Product removed from wishlist successfully.",
            null));
    }
}