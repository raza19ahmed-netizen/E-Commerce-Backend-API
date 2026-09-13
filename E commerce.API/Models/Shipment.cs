using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.Models;

public class Shipment
{
    public int Id { get; set; }

    // जिस Order की shipment है
    public int OrderId { get; set; }

    public Order? Order { get; set; }

    // Tracking number
    [Required]
    [MaxLength(100)]
    public string TrackingNumber { get; set; } = string.Empty;

    // Pending → Processing → Shipped → OutForDelivery → Delivered
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Processing";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
}