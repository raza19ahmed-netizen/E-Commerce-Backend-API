using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Review> AddAsync(Review review)//"Service ne mujhe ek naya Review object diya hai. Isko Reviews table me insert karo, database me save karo, phir saved review wapas do."
    {
        _context.Reviews.Add(review);//_context = database context...Reviews = DbSet...Add() = Ye EF Core ka method hai...Yahan Service se Review object aa raha hai.
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<bool> ExistsAsync(int userId, int productId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId);//.AnyAsync=Ye EF Core ka method hai. ye user/client our Review ki productId & UserId compare kar rha hai
    }

    public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Include(r => r.User)//Review ke saath related User data bhi load karo."
            .OrderByDescending(r => r.CreatedAt)//Latest reviews pehle dikhne chahiye."
            .ToListAsync();//Ab query ko execute karo aur results ki List banao
    }

    public async Task<(double AverageRating, int TotalReviews)> GetRatingSummaryAsync(int productId)
    {
        var reviews = _context.Reviews
            .Where(r => r.ProductId == productId);

        var totalReviews = await reviews.CountAsync();

        if (totalReviews == 0)
        {
            return (0, 0);
        }

        var averageRating = await reviews.AverageAsync(r => (double)r.Rating);

        return (averageRating, totalReviews);
    }

    public async Task<Review?> GetByIdAndUserIdAsync(
    int reviewId,
    int userId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.Id == reviewId && r.UserId == userId);
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}