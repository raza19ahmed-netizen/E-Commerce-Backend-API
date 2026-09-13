using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IWishlistRepository
{
    Task<WishlistItem> AddAsync(WishlistItem wishlistItem);

    Task<bool> ExistsAsync(int userId, int productId);

    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);

    Task<WishlistItem?> GetByIdAndUserIdAsync(int wishlistItemId, int userId);

    Task DeleteAsync(WishlistItem wishlistItem);
}