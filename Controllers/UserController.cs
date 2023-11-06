using ECommerceApp.Database;
using ECommerceApp.Errors;
using ECommerceApp.Models;
using ECommerceApp.Services;
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
        public UserController(ApplicationDBContext dbContext, TokenService tokenService, SendMail sendMail)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // This is a null check.
            _tokenService = tokenService;
            _sendMail = sendMail;
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
    }


}

