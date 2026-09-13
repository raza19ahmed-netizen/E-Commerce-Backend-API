using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.Models;

public class Coupon
{
    public int Id { get; set; }

    // Example: SAVE10, FLAT100
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    // Percentage or Fixed
    [Required]
    [MaxLength(20)]
    public string DiscountType { get; set; } = string.Empty;

    // Example: 10% or ₹100
    public decimal DiscountValue { get; set; }

    // Minimum order amount required to use coupon
    public decimal MinimumOrderAmount { get; set; }

    // Maximum discount amount (useful for percentage coupons)
    public decimal? MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    // How many times this coupon can be used
    public int UsageLimit { get; set; }

    // How many times it has already been used
    public int UsedCount { get; set; } = 0;
}