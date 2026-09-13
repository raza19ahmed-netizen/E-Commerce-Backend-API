
using System.ComponentModel.DataAnnotations;
namespace E_commerce.API.DTOs
{
    public class CreateCategoryDto  // POST api create karte wakt user se kya lena hai...request //
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 charectors.")]
        public string Name { get; set; } = string.Empty;
    }
}
