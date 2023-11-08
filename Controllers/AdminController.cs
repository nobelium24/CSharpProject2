using ECommerceApp.Database;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Errors;
using ECommerceApp.Services;

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

                return Json(new { message = "User created successfully" });

            }
            catch (System.Exception)
            {
                throw;
            }
        }

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

                return Json(new { message = $"Welcome, {admin.UserName}", adminToken = token });
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}