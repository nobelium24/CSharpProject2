
using ECommerceApp.Database;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace ECommerceApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoryController(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpPost]
        [Route("/api/category/createcategory", Name = "CreateCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewCategory([FromBody] CategoryModel categoryModel)
        {
            try
            {
                var admin = User.FindFirst(ClaimTypes.Email)?.Value;
                if(string.IsNullOrEmpty(admin))
                    return Unauthorized("No email claims found in token");

                var verifyAdmin = await _dbContext.Admin
                .FirstOrDefaultAsync(u => u.Email == admin) ?? throw new UserNotFoundException();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var verifyCategory = _dbContext.Categories.Any(c => c.CategoryName == categoryModel.CategoryName);
                if (verifyCategory)
                    throw new CategoryAlreadyExistException(categoryModel.CategoryName);

                var category = new CategoryModel
                {
                    CategoryName = categoryModel.CategoryName,
                    CategoryDescription = categoryModel.CategoryDescription
                };

                _dbContext.Categories.Add(category);
                await _dbContext.SaveChangesAsync();

                return StatusCode(201, new { message = "Category created successfully" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/category/getcategories", Name = "GetCategory")]
        public IActionResult GetAllCategories()
        {
            try
            {
                var catrgories = _dbContext.Categories.ToList();
                return StatusCode(200, catrgories);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/category/getcategory/{categoryId}", Name = "GetSingleCategory")]
        public IActionResult GetSingleCategory(int categoryId)
        {
            try
            {
                CategoryModel category = _dbContext.Categories.SingleOrDefault(c => c.CategoryId == categoryId) ?? throw new CategoryDoesNotExistException();

                return StatusCode(200, category);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/category/getcategorybyname", Name = "GetCategoryByName")]
        public IActionResult GetSingleCategoryByName([FromBody] string categoryName)
        {
            try
            {
                CategoryModel category = _dbContext.Categories.SingleOrDefault(c => c.CategoryName == categoryName) ?? throw new CategoryDoesNotExistException();

                return StatusCode(200, category);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        [Route("/api/category/deletecategory{id}", Name = "DeleteCategory")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var admin = User.FindFirst(ClaimTypes.Email)?.Value;
                if(string.IsNullOrEmpty(admin))
                    return Unauthorized("No valid email claims in token");
                
                var verifyAdmin = await _dbContext.Admin
                .FirstOrDefaultAsync(a => a.Email == admin) ?? throw new UserNotFoundException();

                CategoryModel category = _dbContext.Categories.SingleOrDefault(c => c.CategoryId == id) ?? throw new CategoryDoesNotExistException();
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new { message = "Category deleted successfully" });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPatch]
        [Route("/api/category/updatecategory/{id}", Name = "UpdateCategory")]
        [Authorize(Roles = "Admin")]
        //install JsonPatch dotnet add package Microsoft.AspNetCore.JsonPatch --version 8.0.0-rc.2.23480.2
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] JsonPatchDocument<CategoryModel> patchDocument)
        {
            try
            {
                var admin = User.FindFirst(ClaimTypes.Email)?.Value;
                if(string.IsNullOrEmpty(admin))
                    return Unauthorized("No valid email claims in token");

                var verifyAdmin = await _dbContext.Admin 
                .FirstOrDefaultAsync(a => a.Email == admin) ?? throw new UserNotFoundException();

                CategoryModel categoryModel = _dbContext.Categories.SingleOrDefault(c => c.CategoryId == id) ?? throw new CategoryDoesNotExistException();

                patchDocument.ApplyTo(categoryModel);
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new {message = "Category updated successfully"});
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}