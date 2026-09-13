namespace E_commerce.API.DTOs;

public class ShipmentDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
}