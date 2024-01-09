using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductId { get; set; }
        public string? PublicId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
    }
}