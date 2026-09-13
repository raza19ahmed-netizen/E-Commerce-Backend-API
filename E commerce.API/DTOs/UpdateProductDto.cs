using System.ComponentModel.DataAnnotations;

namespace E_commerce.API.DTOs
{
    public class UpdateProductDto // PUT Api update karteb wakt user se ky lena hai ....request //
    {
        [Range(1, int.MaxValue )]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 charectors.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 10000000 , ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, 1000000 , ErrorMessage = "Stock can't be negative.")]
        public int Stock { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please provide valid CategoryId.")]
        public int CategoryId { get; set; }
    }
}