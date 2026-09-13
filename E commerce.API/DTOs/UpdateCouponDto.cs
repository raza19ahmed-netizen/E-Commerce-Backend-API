using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class UpdateCouponDto
{
    [Required]
    public string DiscountType { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Range(1, int.MaxValue)]
    public int UsageLimit { get; set; }

    public bool IsActive { get; set; }
}