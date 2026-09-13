namespace E_commerce.API.Models;

public class CouponUsage
{
    public int Id { get; set; }

    // किस User ने coupon use किया
    public int UserId { get; set; }

    public User? User { get; set; }

    // कौन सा Coupon use किया
    public int CouponId { get; set; }

    public Coupon? Coupon { get; set; }

    // Coupon कितनी बार इस user ने use किया
    public int UsageCount { get; set; } = 0;

    // आखिरी बार कब use किया
    public DateTime LastUsedAt { get; set; }
}