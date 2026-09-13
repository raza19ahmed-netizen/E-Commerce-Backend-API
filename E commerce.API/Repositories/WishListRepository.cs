using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly ApplicationDbContext _context;

    public WishlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WishlistItem> AddAsync(WishlistItem wishlistItem)
    {
        _context.WishlistItems.Add(wishlistItem);
        await _context.SaveChangesAsync();

        return wishlistItem;
    }

    public async Task<bool> ExistsAsync(int userId, int productId)
    {
        return await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId &&
                           w.ProductId == productId);
    }

    public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId)
    {
        return await _context.WishlistItems
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<WishlistItem?> GetByIdAndUserIdAsync(
        int wishlistItemId,
        int userId)
    {
        return await _context.WishlistItems
            .FirstOrDefaultAsync(w =>
                w.Id == wishlistItemId &&
                w.UserId == userId);
    }

    public async Task DeleteAsync(WishlistItem wishlistItem)
    {
        _context.WishlistItems.Remove(wishlistItem);
        await _context.SaveChangesAsync();
    }
}