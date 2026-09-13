using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class UpdateCartItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}