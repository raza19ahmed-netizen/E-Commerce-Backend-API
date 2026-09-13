using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class CreateShipmentDto
{
    [Required]
    [MaxLength(100)]
    public string TrackingNumber { get; set; } = string.Empty;
}