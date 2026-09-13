
using System.ComponentModel.DataAnnotations;
namespace E_commerce.API.DTOs
{
    public class UpdateCategoryDto // PUT Api update karteb wakt user se ky lena hai ....request //
    {
        [Required(ErrorMessage = "Category name is required")]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 charectors.")]
        public string Name { get; set; } = string.Empty;
    }
}
