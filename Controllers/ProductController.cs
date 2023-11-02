using CloudinaryDotNet;
using ECommerceApp.Configuration;
using ECommerceApp.Database;
using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq;
// using ECommerceApp.Services;

namespace ECommerceApp.Controllers
{

    public class ProductController : Controller
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly ApplicationDBContext _dbContext;

        
        public ProductController(CloudinaryService cloudinaryService, ApplicationDBContext dbContext)
        {
            _cloudinaryService = cloudinaryService;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("/api/product/uploadproduct", Name = "UploadProduct")]
        public async Task<IActionResult> UploadProduct([FromBody] ProductModel productModel)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                // var cloudinaryService = new CloudinaryService(_appCloudinaryConfiguration);
                var imageUrl = await _cloudinaryService.UploadBase64Media(productModel.ProductImage);

                var product = new ProductModel
                {
                    ProductName = productModel.ProductName,
                    ProductDescription = productModel.ProductDescription,
                    ProductPrice = productModel.ProductPrice,
                    ProductQuantity = productModel.ProductQuantity,
                    ProductImage = imageUrl,
                    CategoryId = productModel.CategoryId,
                    // Category = await _dbContext.Categories.FirstAsync(c => c.CategoryId == productModel.CategoryId)
                };

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                return Json(new { message = "Product created successfully" });

            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }

}