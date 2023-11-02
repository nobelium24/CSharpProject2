using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class CartModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [Required(ErrorMessage = "ProductId is required")]
        public required int Quantity { get; set; }

        public int ProductId { get; set; } // This is a foreign key.
        [ForeignKey("ProductId")]

        public required ProductModel Product { get; set; } // This is a navigation property.

        public int UserId { get; set; } // This is a foreign key.
        [ForeignKey("UserId")]

        public required UserModel User { get; set; } // This is a navigation property.

        
    }
}