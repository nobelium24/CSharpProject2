using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Password can only contain letters and numbers")]
        public string ? Password { get; set; }
    }
}