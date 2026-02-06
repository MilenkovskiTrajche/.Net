using System.ComponentModel.DataAnnotations;

namespace EShop.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]

        public string Description { get; set; } = string.Empty;
        [Required]

        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public Guid? CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
