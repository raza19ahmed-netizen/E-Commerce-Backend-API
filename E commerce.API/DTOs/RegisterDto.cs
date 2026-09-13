using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class RegisterDto    // user register ke time client se data lene ke liye bnaya gya hai//
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
