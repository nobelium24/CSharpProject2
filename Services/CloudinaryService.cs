
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerceApp.Configuration;

namespace ECommerceApp.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
            configuration["Cloudinary:CloudName"],
            configuration["Cloudinary:ApiKey"],
            configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadBase64Media(string base64Media)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription("image", new MemoryStream(Convert.FromBase64String(base64Media))),
                    PublicId = Guid.NewGuid().ToString(),
                    Folder = "my_folder"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}