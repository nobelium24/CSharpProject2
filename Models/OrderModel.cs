using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class OrderModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public int SellerId { get; set; }
        public string? Reference { get; set; }

        [EnumDataType(typeof(Status))]
        public Status? Status { get; set; }

        [ForeignKey("UserId")]
        public UserModel? User { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }

        [ForeignKey("SellerId")]
        public UserModel? Seller { get; set; }
    }

    public enum Status
    {
        Pending,
        Accepted,
        Rejected,
        Delivered
    }
}