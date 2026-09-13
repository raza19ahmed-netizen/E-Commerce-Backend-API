using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs;

public class CreateReviewDto // review banane ke liyen kya kya chahiye
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; } = string.Empty;
}