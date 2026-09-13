using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;

    public WishlistService(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
    }

    // ❤️ Product को Wishlist में Add करना
    public async Task<WishlistItemDto> AddAsync(int userId, int productId)
    {
        // Step 1: Product मौजूद है या नहीं check करो
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Step 2: Duplicate Wishlist item check करो
        var alreadyExists = await _wishlistRepository
            .ExistsAsync(userId, productId);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "This product is already in your wishlist.");
        }

        // Step 3: नया WishlistItem बनाओ
        var wishlistItem = new WishlistItem
        {
            UserId = userId,
            ProductId = productId
        };

        // Step 4: Database में Save करो
        var savedItem = await _wishlistRepository.AddAsync(wishlistItem);

        // Step 5: Clean DTO Response बनाओ
        return new WishlistItemDto
        {
            Id = savedItem.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            CreatedAt = savedItem.CreatedAt
        };
    }

    // 📋 Logged-in User की पूरी Wishlist प्राप्त करना
    public async Task<IEnumerable<WishlistItemDto>> GetMyWishlistAsync(int userId)
    {
        var wishlistItems = await _wishlistRepository
            .GetByUserIdAsync(userId);

        return wishlistItems.Select(item => new WishlistItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            CreatedAt = item.CreatedAt
        }).ToList();
    }

    // 🗑️ Wishlist से Item हटाना
    public async Task RemoveAsync(int wishlistItemId, int userId)
    {
        // सिर्फ logged-in user का अपना Wishlist Item खोजो
        var wishlistItem = await _wishlistRepository
            .GetByIdAndUserIdAsync(wishlistItemId, userId);

        if (wishlistItem == null)
        {
            throw new KeyNotFoundException(
                "Wishlist item not found or you do not have permission to remove it.");
        }

        await _wishlistRepository.DeleteAsync(wishlistItem);
    }
}