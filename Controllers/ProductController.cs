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
using CloudinaryDotNet.Actions;
// using ECommerceApp.Services;
using System.Security.Claims;

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
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("No email claims found in token");

                var verifyUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // var cloudinaryService = new CloudinaryService(_appCloudinaryConfiguration);
                // var imageUrl = await _cloudinaryService.UploadBase64Media(productModel.ProductImage ?? throw new IsNullException());
                List<ImageModel> imageUrls = new();
                foreach (var image in productModel.Images ?? throw new IsNullException())
                {
                    var imageUrl = await _cloudinaryService.UploadBase64Media(image.ImageUrl ?? throw new IsNullException());
                    imageUrls.Add(imageUrl);
                }


                var product = new ProductModel
                {
                    ProductName = productModel.ProductName,
                    ProductDescription = productModel.ProductDescription,
                    ProductPrice = productModel.ProductPrice,
                    ProductQuantity = productModel.ProductQuantity,
                    // ProductImage = imageUrl,
                    CategoryId = productModel.CategoryId,
                    // Category = await _dbContext.Categories.FirstAsync(c => c.CategoryId == productModel.CategoryId)
                };

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                int productId = product.ProductId;

                foreach (var imageUrl in imageUrls)
                {
                    var image = new ImageModel
                    {
                        ImageUrl = imageUrl.ImageUrl,
                        ProductId = productId,
                        PublicId = imageUrl.PublicId
                    };
                    _dbContext.Images.Add(image);
                    await _dbContext.SaveChangesAsync();
                }

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
                var products = _dbContext.Products
                .Include(p => p.Images)
                .Select(p => new ProductModel
                {
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    // ProductImage = p.ProductImage
                    Images = p.Images.Select(i => new ImageModel
                    {
                        ImageUrl = i.ImageUrl,
                        ProductId = i.ProductId
                    }).Where(i => i != null).ToList()
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
                var product = await _dbContext.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductName == model.ProductName) ?? throw new IsNullException();
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
                var product = await _dbContext.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id) ?? throw new IsNullException();
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
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("No email claims found in token");

                var verifyUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                ProductModel productModel = _dbContext.Products
                .SingleOrDefault(c => c.CategoryId == id) ?? throw new IsNullException();

                if (productModel == null) return NotFound("Id does not exist");

                var updateProduct = new ProductModel
                {
                    ProductId = productModel.ProductId,
                    ProductName = productModel.ProductName,
                    ProductDescription = productModel.ProductDescription,
                    ProductPrice = productModel.ProductPrice,
                    // ProductImage = productModel.ProductImage,
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
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("No email claims found in token");

                var verifyUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var imageModel = await _dbContext.Images
                .Where(i => i.ProductId == id)
                .ToListAsync() ?? throw new IsNullException();

                foreach (var image in imageModel)
                {
                    var deleteParams = new DeletionParams(image.PublicId);
                    var deletionResult = await _cloudinaryService
                    .DeleteMedia(deleteParams.PublicId ?? throw new IsNullException());
                    if (deletionResult != "ok") throw new ImageDeleteException();
                    _dbContext.Images.Remove(image);
                }

                var productModel = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == id) ?? throw new IsNullException();

                _dbContext.Products.Remove(productModel);
                await _dbContext.SaveChangesAsync();
                return Ok("Product deleted");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("/api/product/editproductimage/{productid}", Name = "EditProductImage")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProductImage(int productid, [FromBody] List<ImageModel> imageModel)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("No email claims found in token");

                var verifyUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email) ?? throw new UserNotFoundException();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                List<ImageModel> images = await _dbContext.Images
                .Where(p => p.ProductId == productid)
                .ToListAsync() ?? throw new IsNullException();

                foreach (var image in images)
                {
                    var deleteParams = new DeletionParams(image.PublicId);
                    var deletionResult = await _cloudinaryService
                    .DeleteMedia(deleteParams.PublicId ?? throw new IsNullException());
                    if (deletionResult != "ok") throw new ImageDeleteException();
                    _dbContext.Images.Remove(image);
                }

                foreach (var image in imageModel)
                {
                    var imageUrl = await _cloudinaryService.UploadBase64Media(image.ImageUrl ?? throw new IsNullException());
                    var newImage = new ImageModel
                    {
                        ImageUrl = imageUrl.ImageUrl,
                        ProductId = productid,
                        PublicId = imageUrl.PublicId
                    };
                    _dbContext.Images.Add(newImage);
                    await _dbContext.SaveChangesAsync();
                }

                return StatusCode(200, new { message = "Product image updated successfully" });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }

}