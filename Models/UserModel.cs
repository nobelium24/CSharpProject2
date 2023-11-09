using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; // This is needed for the [IndexAttribute] below.

namespace ECommerceApp.Models
{
    public class UserModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can only contain letters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name can only contain letters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Password can only contain letters and numbers")]
        public required string Password { get; set; }

        public bool IsSeller { get; set; }

        public bool IsScammer { get; set; }

        public string? StoreName { get; set; }

        public string? StoreDescription { get; set; }
        public UserModel()
        {
            IsSeller = false; // This initializes the IsSeller property to false.
            IsScammer = false;
            Carts = new List<CartModel>(); // This initializes the Carts property to an empty list.
        }

        public List<CartModel> Carts { get; set; }


        //For error validation messages. The method below is used to get the validation errors for the model. It returns a dictionary with the property name as the key and the error message as the value.
        // public Dictionary<string, string> GetValidationErrors()
        // {
        //     var validationErrors = new Dictionary<string, string>(); // This creates a new dictionary to hold the errors.
        //     var validationContext = new ValidationContext(this); // This creates a validation context for the model. We are using "this" to refer to the current instance of the model.
        //     var validationResults = new List<ValidationResult>(); // This creates a list to hold the validation results.

        //     if (!Validator.TryValidateObject(this, validationContext, validationResults, true)) // This validates the model. The "true" is for the validateAllProperties parameter. If it is set to true, then all properties will be validated. If it is set to false, then only the required properties will be validated.
        //     {
        //         foreach (var validationResult in validationResults)
        //         {
        //             if (validationResult.ErrorMessage != null)
        //             {
        //                 validationErrors.Add(validationResult.MemberNames.First(), validationResult.ErrorMessage); // This adds the error message to the dictionary. The MemberNames.First() is the name of the property that the error is associated with. For example, if the error is for the FirstName property, then the key will be "FirstName" and the value will be the error message.

        //             }
        //         }
        //     }

        //     return validationErrors;

        // }
    }
}