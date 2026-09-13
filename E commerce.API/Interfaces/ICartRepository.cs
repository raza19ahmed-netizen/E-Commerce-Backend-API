using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(int userId);// user ka existing cart dhoondna//

    Task<Cart> AddCartAsync(Cart cart); //new cart bnana//

    Task<CartItem?> GetCartItemAsync(int cartId, int productId);// check karna ki product phle se cart me add hai ki nhi// 

    Task<CartItem> AddCartItemAsync(CartItem cartItem); // new product cart me add karna //

    Task UpdateCartItemAsync(CartItem cartItem);// existing product ki quantity update// 

    Task<Cart?> GetCartWithItemsAsync(int userId);//Mujhe kisi particular user ka Cart database se nikalna hai, aur us Cart ke andar ke saare CartItems bhi chahiye."

    Task RemoveCartItemAsync(CartItem cartItem);//

    Task ClearCartAsync(int cartId);//Mujhe ek aisa method define karna hai jo diye gaye CartId ke Cart ko clear kare aur koi value return na kare.

}