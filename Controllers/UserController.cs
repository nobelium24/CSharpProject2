using ECommerceApp.Database;
using ECommerceApp.Errors;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;


namespace ECommerceApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly TokenService _tokenService;

        private readonly SendMail _sendMail;

        private readonly CodeGenerator _codeGenerator;
        public UserController(ApplicationDBContext dbContext, TokenService tokenService, SendMail sendMail, CodeGenerator codeGenerator)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // This is a null check.
            _tokenService = tokenService;
            _sendMail = sendMail;
            _codeGenerator = codeGenerator;
        }

        [HttpPost]
        [Route("/api/user/register", Name = "RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserModel userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var verifyUser = _dbContext.Users.Any(u => u.Email == userModel.Email);
                if (verifyUser)
                    throw new UserAlreadyExistException(userModel.Email);

                var user = new UserModel()
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    Password = userModel.Password,
                };

                var hashedPassword = new PasswordHasher<UserModel>().HashPassword(user, user.Password);
                user.Password = hashedPassword;
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _sendMail.SendEmail(userModel.Email, userModel.FirstName ?? throw new IsNullException());

                return Json(new { message = "User created successfully" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/user/login", Name = "LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userModel.Email) ?? throw new UserNotFoundException();

                if (userModel.Password == null)
                    return BadRequest(new { message = "Password cannot be null" });

                var passwordVerificationResult = new PasswordHasher<UserModel>().VerifyHashedPassword(user, user.Password, userModel.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                    return BadRequest(new { message = "Invalid email or password" });

                var token = "";
                if (user.IsSeller)
                {
                    token = _tokenService.GenerateSellerToken(user.Email, "Seller");
                }
                else
                {
                    token = _tokenService.GenerateToken(user.Email);
                }

                if (token == "")
                    throw new AuthorizationException();

                return Json(new { message = $"Welcome, {user.FirstName}", userToken = token });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/user/test")]
        [Authorize]
        public IActionResult Get()
        {
            // var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // if(StringValues.IsNullOrEmpty(token))
            //     return Unauthorized();
            // try
            // {
            //     TokenObject user = _tokenService.GetUserFromToken(token ?? throw new IsNullException());
            //     return Ok(user.DecodedToken);
            // }
            // catch(SecurityTokenException exception)
            // {
            //     return Unauthorized(exception.Message);
            // }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("No email claims found in token");

            return Ok(email);
        }

        [HttpPost]
        [Route("/api/user/forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserModel userModel)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userModel.Email) ?? throw new UserNotFoundException();
                string code = _codeGenerator.VerificationCode(4);
                var forgottenPassword = new ForgotPasswordModel()
                {
                    Email = userModel.Email,
                    VerificationCode = code
                };
                _dbContext.ForgotPassword.Add(forgottenPassword);
                await _dbContext.SaveChangesAsync();
                var firstName = user.FirstName ?? throw new IsNullException();

                _sendMail.SendForgotPasswordMail(userModel.Email, firstName, code);
                return Json(new { message = "Check your mail for verification code" });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/user/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordModel resetPassword)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == resetPassword.Email) ?? throw new UserNotFoundException();
                var verifyUser = await _dbContext.ForgotPassword.FirstOrDefaultAsync(u => u.VerificationCode == resetPassword.VerificationCode) ?? throw new InvalidResetPasswordCodeException();

                var newPassword = resetPassword.NewPassword ?? throw new IsNullException();
                var hashNewPassword = new PasswordHasher<UserModel>().HashPassword(user, resetPassword.NewPassword);

                user.Password = hashNewPassword;

                await _dbContext.SaveChangesAsync();

                _dbContext.ForgotPassword.Remove(verifyUser);

                return Json(new { message = "Password has been reset successfully." });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/user/findstore")]
        public async Task<IActionResult> FindStore([FromBody] UserModel model)
        {
            try
            {
                var store = await _dbContext.Users.FirstOrDefaultAsync(s => s.StoreName == model.StoreName) ?? throw new UserNotFoundException();
                return Ok(store);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/user/addtocart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] ProductModel model)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var verifyUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == model.ProductId) ?? throw new ProductNotFoundException();

                var cartItem = new CartModel()
                {
                    Quantity = model.ProductQuantity,
                    UserId = verifyUser.Id,
                    ProductId = product.ProductId
                };
                _dbContext.Carts.Add(cartItem);
                await _dbContext.SaveChangesAsync();
                return Ok("Item added to cart successfully");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        [Route("/api/user/removefromcart/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var verifyUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                var cartItem = await _dbContext.Carts.FirstOrDefaultAsync(c => c.CartId == id) ?? throw new ProductNotFoundException();
                _dbContext.Carts.Remove(cartItem);
                await _dbContext.SaveChangesAsync();
                return Ok("Item removed from cart");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/user/viewcart")]
        [Authorize]
        public async Task<IActionResult> ViewCart()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var verifyUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                var cartItems = await _dbContext.Carts
                    .Where(c => c.UserId == verifyUser.Id)
                    .Include(c => c.Product)
                    .Select(c => new {
                        Product = c.Product,
                        Quantity = c.Quantity
                    }).ToListAsync();
                
                return Ok(cartItems);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }


}

