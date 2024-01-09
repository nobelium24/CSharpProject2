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
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "ProductName can only contain letters and numbers")]
        [StringLength(50, ErrorMessage = "ProductName cannot be longer than 50 characters")]
        public string ? ProductName { get; set; }

        [Required(ErrorMessage = "ProductDescription is required")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "ProductDescription can only contain letters and numbers")]
        [StringLength(500, ErrorMessage = "ProductDescription cannot be longer than 500 characters")]
        public string ? ProductDescription { get; set; }

        [Required(ErrorMessage = "ProductPrice is required")]
        [Range(0, 1000000, ErrorMessage = "ProductPrice must be between 0 and 1000000")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "ProductQuantity is required")]
        [Range(0, 1000000, ErrorMessage = "ProductQuantity must be between 0 and 1000000")]
        public int ProductQuantity { get; set; }

        public int CategoryId { get; set; } // This is a foreign key.

        public int UserId { get; set; } // This is a foreign key.

        [ForeignKey("CategoryId")]
        public CategoryModel ? Category { get; set; } // This is a navigation property.

        public ICollection<ImageModel> ? Images { get; set; }

        public ICollection<CartModel> ? Carts { get; set; }

        public ICollection<OrderModel> ? Orders { get; set; }

        [ForeignKey("UserId")]
        public UserModel ? User { get; set; } // This is a navigation property.

    }
}