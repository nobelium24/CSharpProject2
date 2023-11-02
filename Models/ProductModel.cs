using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class ProductModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "ProductName is required")]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "ProductDescription is required")]
        public required string ProductDescription { get; set; }

        [Required(ErrorMessage = "ProductPrice is required")]
        public required int ProductPrice { get; set; }

        [Required(ErrorMessage = "ProductQuantity is required")]
        public required int ProductQuantity { get; set; }

        [Required(ErrorMessage = "ProductImage is required")]
        public required string ProductImage { get; set; }

        public required int CategoryId { get; set; } // This is a foreign key.

        [ForeignKey("CategoryId")]
        public CategoryModel ? Category { get; set; } // This is a navigation property.

        public List<CartModel> ? Carts { get; set; }

    }
}