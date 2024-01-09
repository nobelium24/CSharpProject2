namespace ECommerceApp.Errors
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found") { }
    }

    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string email) : base($"email {email} already in use") { }
    }

    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base("Authorization error") { }
    }

    public class CategoryAlreadyExistException : Exception
    {
        public CategoryAlreadyExistException(string category) : base($"Category {category} already exists") { }
    }

    public class CategoryDoesNotExistException : Exception
    {
        public CategoryDoesNotExistException() : base("Category does not exist") { }
    }

    public class IsNullException : Exception
    {
        public IsNullException() : base("Variable is null") { }
    }

    public class InvalidResetPasswordCodeException : Exception
    {
        public InvalidResetPasswordCodeException() : base("Password reset code is invalid") { }
    }

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() : base("User not found") { }
    }

    public class NullIpAddressException: Exception
    {
        public NullIpAddressException() : base("IpAddress is not valid"){}
    }

    public class ImageDeleteException : Exception
    {
        public ImageDeleteException() : base("Image could not be deleted") { }
    }
}