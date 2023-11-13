using Azure;
using CloudinaryDotNet;
using ECommerceApp.Configuration;
using ECommerceApp.Database;
using ECommerceApp.Errors;
using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UploadProduct([FromBody] ProductModel productModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // var cloudinaryService = new CloudinaryService(_appCloudinaryConfiguration);
                var imageUrl = await _cloudinaryService.UploadBase64Media(productModel.ProductImage ?? throw new IsNullException());

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

                return StatusCode(201, new { message = "Product created successfully" });

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/product/fetchproducts", Name = "FetchProduct")]
        public ActionResult FetchAllProducts()
        {
            try
            {
                var products = _dbContext.Products.Select(p => new ProductModel
                {
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductImage = p.ProductImage
                }).ToList();
                return StatusCode(200, new { message = "Products retrieved successfully", products });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("/api/product/getsingleproduct", Name = "GetSingleProduct")]
        public async Task<IActionResult> GetSingleProduct([FromBody] ProductModel model)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == model.ProductName) ?? throw new IsNullException();
                return Ok(product);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("/api/product/getproductbyid/{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id) ?? throw new IsNullException();
                return Ok(product);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPatch]
        [Route("/api/product/updateproduct/{id}", Name = "UpdateProduct")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] JsonPatchDocument<ProductModel> updatedProduct)
        {
            try
            {
                ProductModel productModel = _dbContext.Products.SingleOrDefault(c => c.CategoryId == id) ?? throw new IsNullException();
                if (productModel == null) return NotFound("Id does not exist");

                var updateProduct = new ProductModel
                {
                    ProductId = productModel.ProductId,
                    ProductName = productModel.ProductName,
                    ProductDescription = productModel.ProductDescription,
                    ProductPrice = productModel.ProductPrice,
                    ProductImage = productModel.ProductImage,
                    ProductQuantity = productModel.ProductQuantity
                };

                updatedProduct.ApplyTo(updateProduct);

                _dbContext.Entry(productModel).CurrentValues.SetValues(updatedProduct);
                await _dbContext.SaveChangesAsync();

                return StatusCode(200, new { message = "Product updated successfully" });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        [Route("/api/product/deleteproduct/{id}", Name = "DeleteProduct")]
        [Authorize(Roles = "Seller, Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var productModel = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id) ?? throw new IsNullException();
                _dbContext.Products.Remove(productModel);
                await _dbContext.SaveChangesAsync();
                return Ok("Product deleted");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }

}