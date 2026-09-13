using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class UpdateOrderStatusDto
{
    [Required]
    [RegularExpression(
        "^(Pending|Confirmed|Shipped|Delivered)$",
        ErrorMessage = "Status must be Pending, Confirmed, Shipped, or Delivered.")]
    public string Status { get; set; } = string.Empty;
}