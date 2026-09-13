using E_commerce.API.DTOs; // mjhe AddToCartDto class use karni hai,jo DTO Folder/nameSpace me hai//

namespace E_commerce.API.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(int userId, AddToCartDto dto); //Cart me product add Karne ke liye mujhe User ki ID aur client se aaya hua product/quantity data Chahiye//

    Task<CartDto> GetCartAsync(int userId);

    Task RemoveFromCartAsync(int userId, int productId);

    Task UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto dto);

    Task ClearCartAsync(int useId);

}
