using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class CategoryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "CategoryName is required")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "CategoryName can only contain letters and numbers")]
        [StringLength(50, ErrorMessage = "CategoryName cannot be longer than 50 characters")]
        public required string CategoryName { get; set; }

        [Required(ErrorMessage = "CategoryDescription is required")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "CategoryDescription can only contain letters and numbers")]
        [StringLength(500, ErrorMessage = "CategoryDescription cannot be longer than 500 characters")]
        public required string CategoryDescription { get; set; }

        public List<ProductModel>? Products { get; set; }
    }
}