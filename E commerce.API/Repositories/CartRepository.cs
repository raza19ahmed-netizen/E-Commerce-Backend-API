using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context) // Jab cartRepository banega mjhe ApplicationDbContext dedena //
    {
        _context = context;
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId)   // mjhe user ka cart Chahiye //
    {
        return await _context.Carts                           // Cart table me search karo //
            .FirstOrDefaultAsync(c => c.UserId == userId);   // jis Cart ka UserId,diye gaye userId ke equal hai ,vo cart dhondo//
    }

    public async Task<Cart> AddCartAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task<CartItem?> GetCartItemAsync(
        int cartId, int productId)
    {
        return await _context.CartItems //mjhe exactly isi Cart ka aur isi Product Ka CartItem chahiye//
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cartId &&
                ci.ProductId == productId);
    }

    public async Task<CartItem> AddCartItemAsync(CartItem cartItem) 
    {
        await _context.CartItems.AddAsync(cartItem); // agar product cart me nahin hai to nasya cartitem create karke database me add karo//
        await _context.SaveChangesAsync();

        return cartItem;
    }

    public async Task UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem); // cartitem phle se exist karta hai,ab uski information update karni hai//
        await _context.SaveChangesAsync();
    }

    public async Task<Cart?> GetCartWithItemsAsync(int userId)// Mujhe userId diya gaya hai. Us user ka Cart database se dhoondo, aur Cart ke andar ke saare CartItems bhi saath me lekar aao."
    {

        return await _context.Carts
            .Include(c => c.CartItems) //"Sirf Cart mat lana. Cart ke andar ke CartItems bhi saath me load karna."
            .ThenInclude(ci => ci.Product) //"CartItems mil gaye, lekin mujhe har CartItem ka Product bhi chahiye
            .FirstOrDefaultAsync(c => c.UserId == userId);//Ab Carts me se wo pehla Cart lao jiska UserId requested userId ke equal hai."

    }

    public async Task RemoveCartItemAsync(CartItem cartItem)
    {

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int cartId)//Diye gaye cartId ke saare CartItems database se nikalo, un sabko delete karo aur changes database me save kar do
    {
        var cartItems = await _context.CartItems//Mujhe CartItems table se data chahiye."
            .Where(ci => ci.CartId == cartId) //Sirf us Cart ke items nikaalo jiska CartId requested cartId ke equal hai."
            .ToListAsync(); //Database se matching items lekar List bana do

        _context.CartItems.RemoveRange(cartItems);//"List me jitne bhi CartItems hain, sabko delete ke liye mark karo."

        await _context.SaveChangesAsync();//Ab jo deletion maine mark kiya tha, usko actual database me apply/save karo."
    }
}