using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;

    public CouponService(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<CouponDto> CreateAsync(CreateCouponDto dto)
    {
        var code = dto.Code.Trim().ToUpper();//"User coupon code save20, Save20, ya save20 kisi bhi format mein bhej sakta hai. Database mein standard format rakhna hai."

        // Duplicate coupon code check...Database mein pehle check karo ki SAVE20 already exist karta hai ya nahi.
        var existingCoupon =
            await _couponRepository.GetByCodeAsync(code);

        if (existingCoupon != null)
        {
            throw new InvalidOperationException(
                "Coupon code already exists.");
        }

        // Discount type validation
        if (dto.DiscountType != "Percentage" &&
            dto.DiscountType != "Fixed")
        {
            throw new InvalidOperationException(
                "Discount type must be Percentage or Fixed.");
        }

        // Percentage validation
        if (dto.DiscountType == "Percentage" &&
            dto.DiscountValue > 100)
        {
            throw new InvalidOperationException(
                "Percentage discount cannot be greater than 100.");
        }

        // Date validation
        if (dto.ExpiryDate <= dto.StartDate)
        {
            throw new InvalidOperationException(
                "Expiry date must be greater than start date.");
        }

        var coupon = new Coupon
        {
            Code = code,
            DiscountType = dto.DiscountType,
            DiscountValue = dto.DiscountValue,
            MinimumOrderAmount = dto.MinimumOrderAmount,
            MaximumDiscountAmount = dto.MaximumDiscountAmount,
            StartDate = dto.StartDate,
            ExpiryDate = dto.ExpiryDate,
            UsageLimit = dto.UsageLimit,
            IsActive = true,
            UsedCount = 0
        };

        var savedCoupon =
            await _couponRepository.AddAsync(coupon);

        return MapToDto(savedCoupon);
    }

    public async Task<IEnumerable<CouponDto>> GetAllAsync()
    {
        var coupons = await _couponRepository.GetAllAsync();

        return coupons.Select(MapToDto);
    }

    public async Task<CouponDto> GetByIdAsync(int id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);

        if (coupon == null)
        {
            throw new KeyNotFoundException("Coupon not found.");
        }

        return MapToDto(coupon);
    }

    public async Task<CouponDto> UpdateAsync(
        int id,
        UpdateCouponDto dto)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);

        if (coupon == null)
        {
            throw new KeyNotFoundException("Coupon not found.");
        }

        // Discount type validation
        if (dto.DiscountType != "Percentage" &&
            dto.DiscountType != "Fixed")
        {
            throw new InvalidOperationException(
                "Discount type must be Percentage or Fixed.");
        }

        // Percentage validation
        if (dto.DiscountType == "Percentage" &&
            dto.DiscountValue > 100)
        {
            throw new InvalidOperationException(
                "Percentage discount cannot be greater than 100.");
        }

        // Date validation
        if (dto.ExpiryDate <= dto.StartDate)
        {
            throw new InvalidOperationException(
                "Expiry date must be greater than start date.");
        }

        coupon.DiscountType = dto.DiscountType;
        coupon.DiscountValue = dto.DiscountValue;
        coupon.MinimumOrderAmount = dto.MinimumOrderAmount;
        coupon.MaximumDiscountAmount = dto.MaximumDiscountAmount;
        coupon.StartDate = dto.StartDate;
        coupon.ExpiryDate = dto.ExpiryDate;
        coupon.UsageLimit = dto.UsageLimit;
        coupon.IsActive = dto.IsActive;

        await _couponRepository.UpdateAsync(coupon);

        return MapToDto(coupon);
    }

    private static CouponDto MapToDto(Coupon coupon)
    {
        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            MinimumOrderAmount = coupon.MinimumOrderAmount,
            MaximumDiscountAmount = coupon.MaximumDiscountAmount,
            StartDate = coupon.StartDate,
            ExpiryDate = coupon.ExpiryDate,
            IsActive = coupon.IsActive,
            UsageLimit = coupon.UsageLimit,
            UsedCount = coupon.UsedCount
        };
    }
}