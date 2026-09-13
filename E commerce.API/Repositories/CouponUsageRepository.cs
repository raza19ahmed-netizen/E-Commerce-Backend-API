using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class CouponUsageRepository : ICouponUsageRepository
{
    private readonly ApplicationDbContext _context;

    public CouponUsageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // किसी specific User और Coupon की usage record खोजो
    public async Task<CouponUsage?> GetByUserAndCouponAsync(
        int userId,
        int couponId)
    {
        return await _context.CouponUsages
            .FirstOrDefaultAsync(cu =>
                cu.UserId == userId &&
                cu.CouponId == couponId);
    }

    // नई usage record add करो
    public async Task<CouponUsage> AddAsync(
        CouponUsage couponUsage)
    {
        await _context.CouponUsages.AddAsync(couponUsage);
        await _context.SaveChangesAsync();

        return couponUsage;
    }

    // Existing usage record update करो
    public async Task UpdateAsync(
        CouponUsage couponUsage)
    {
        _context.CouponUsages.Update(couponUsage);
        await _context.SaveChangesAsync();
    }
}