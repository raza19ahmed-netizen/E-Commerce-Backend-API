using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface ICouponUsageRepository
{
    // किसी specific User और Coupon की usage जानकारी
    Task<CouponUsage?> GetByUserAndCouponAsync(
        int userId,
        int couponId);

    // नई usage record database में add करना
    Task<CouponUsage> AddAsync(CouponUsage couponUsage);

    // Existing usage record update करना
    Task UpdateAsync(CouponUsage couponUsage);
}