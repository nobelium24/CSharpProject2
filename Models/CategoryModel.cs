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
        public required string CategoryName { get; set; }

        [Required(ErrorMessage = "CategoryDescription is required")]
        public required string CategoryDescription { get; set; }

        public List<ProductModel>? Products { get; set; }
    }
}