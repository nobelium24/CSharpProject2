using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    public class ForgotPasswordModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        public required string Email { get; set; }

        public required string VerificationCode { get; set; }

        public string ? NewPassword {get; set;}
    }
}