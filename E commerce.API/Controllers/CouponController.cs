using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    // ➕ Create Coupon
    [HttpPost]
    public async Task<IActionResult> CreateCoupon(CreateCouponDto dto)
    {
        var coupon = await _couponService.CreateAsync(dto);

        return Ok(new ApiResponse<CouponDto>(
            true,
            "Coupon created successfully.",
            coupon));
    }

    // 📋 Get All Coupons
    [HttpGet]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await _couponService.GetAllAsync();

        return Ok(new ApiResponse<IEnumerable<CouponDto>>(
            true,
            "Coupons retrieved successfully.",
            coupons));
    }

    // 🔍 Get Coupon By ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCouponById(int id)
    {
        var coupon = await _couponService.GetByIdAsync(id);

        return Ok(new ApiResponse<CouponDto>(
            true,
            "Coupon retrieved successfully.",
            coupon));
    }

    // ✏️ Update Coupon
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCoupon(
        int id,
        UpdateCouponDto dto)
    {
        var coupon = await _couponService.UpdateAsync(id, dto);

        return Ok(new ApiResponse<CouponDto>(
            true,
            "Coupon updated successfully.",
            coupon));
    }
}