using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; } // ye product kis category id ka hai actualy it categorid Foreignkey ki trha kaam karti hai//
        public Category? Category { get; set; }  // ye navigation property hai.. product se directly uski poori category information tak pahuchana,matlab poora categoey object aur ? iska matlab hai catogory object bhi hosakta hai aur null bhi//

        public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();

    }
}
