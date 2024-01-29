using ECommerceApp.Database;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Errors;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerceApp.Controllers
{
    public class AdminControllers : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly SendMail _sendMail;
        private readonly TokenService _tokenService;

        public AdminControllers(ApplicationDBContext dbContext, SendMail sendMail, TokenService tokenService)
        {
            _dbContext = dbContext;
            _sendMail = sendMail;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("/api/admin/register")]
        public async Task<IActionResult> Register([FromBody] AdminModel adminModel)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var verifyAdmin = _dbContext.Admin.Any(a => a.Email == adminModel.Email);
                if (verifyAdmin) throw new UserAlreadyExistException(adminModel.Email ?? throw new IsNullException());

                var admin = new AdminModel()
                {
                    UserName = adminModel.UserName,
                    Email = adminModel.Email,
                    Password = adminModel.Password
                };

                var hashedPassword = new PasswordHasher<AdminModel>().HashPassword(admin, admin.Password ?? throw new IsNullException());
                admin.Password = hashedPassword;
                _dbContext.Admin.Add(admin);
                await _dbContext.SaveChangesAsync();

                _sendMail.SendEmail(adminModel.Email ?? throw new IsNullException(), adminModel.UserName ?? throw new IsNullException());

                return StatusCode(201, new { message = "User created successfully" });

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/admin/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginAdmin)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var admin = await _dbContext.Admin.FirstOrDefaultAsync(a => a.Email == loginAdmin.Email) ?? throw new UserNotFoundException();

                var password = loginAdmin.Password ?? throw new IsNullException();
                var passwordTwo = admin.Password ?? throw new IsNullException();
                var passwordVerificationResult = new PasswordHasher<AdminModel>().VerifyHashedPassword(admin, passwordTwo, password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                    return BadRequest(new { message = "Invalid email or password" });

                var token = _tokenService.GenerateAdminToken(loginAdmin.Email ?? throw new IsNullException(), "Admin") ?? throw new AuthorizationException();

                return StatusCode(200, new { message = $"Welcome, {admin.UserName}", adminToken = token });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/admin/findstore")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> FindStore([FromBody] UserModel model)
        {
            try
            {
                var verify = _dbContext.Admin.Any(u => u.Email == model.Email);
                if (!verify) throw new UserNotFoundException();
                var store = await _dbContext.Users.FirstOrDefaultAsync(s => s.StoreName == model.StoreName) ?? throw new UserNotFoundException();
                return Ok(store);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/admin/blacklistuser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlackListUser(int id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UserNotFoundException();
                var verify = _dbContext.Admin.Any(u => u.Email == email);
                if (!verify) throw new UserNotFoundException();

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException();
                user.IsScammer = true;
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new { message = $"User {user.FirstName} has been blacklisted" });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/admin/unblacklistuser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnBlacklistUser(int id)
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UserNotFoundException();
                var verifyAdmin = _dbContext.Admin.Any(u => u.Email == email);
                if (!verifyAdmin) throw new UserNotFoundException();

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException();
                user.IsScammer = false;
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new{message = $"User {user.FirstName} has been un blacklisted"});
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}