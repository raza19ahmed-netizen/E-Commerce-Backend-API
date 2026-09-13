using E_commerce.API.DTOs;

namespace E_commerce.API.Interfaces;

public interface ICouponService
{
    // Admin नया coupon बनाएगा
    Task<CouponDto> CreateAsync(CreateCouponDto dto);

    // सभी coupons प्राप्त करना
    Task<IEnumerable<CouponDto>> GetAllAsync();

    // ID से coupon प्राप्त करना
    Task<CouponDto> GetByIdAsync(int id);

    // Admin coupon update करेगा
    Task<CouponDto> UpdateAsync(int id, UpdateCouponDto dto);
}
