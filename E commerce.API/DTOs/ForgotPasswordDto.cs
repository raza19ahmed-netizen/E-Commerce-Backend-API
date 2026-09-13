using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}