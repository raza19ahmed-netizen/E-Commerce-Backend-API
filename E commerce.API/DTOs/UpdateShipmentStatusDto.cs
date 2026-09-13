using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class UpdateShipmentStatusDto
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}