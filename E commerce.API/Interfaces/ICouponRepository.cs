using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface ICouponRepository
{
    Task<Coupon> AddAsync(Coupon coupon);

    Task<Coupon?> GetByIdAsync(int id);

    Task<Coupon?> GetByCodeAsync(string code);

    Task<IEnumerable<Coupon>> GetAllAsync();

    Task UpdateAsync(Coupon coupon);
}