using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface IWishlistService
{
    Task<WishlistItemDto> AddAsync(int userId, int productId);

    Task<IEnumerable<WishlistItemDto>> GetMyWishlistAsync(int userId);

    Task RemoveAsync(int wishlistItemId, int userId);
}