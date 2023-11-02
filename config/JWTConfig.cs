// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;

// namespace ECommerceApp.Config
// {
//     public static class JwtAuthenticationExtensions
//     {
//         public static void AddJwtAuthentication(this IServiceCollection services, string issuer, string audience, string secretKey)
//         {
//             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true, 
//                     ValidateAudience = true,
//                     ValidateLifetime = true,
//                     ValidateIssuerSigningKey = true,
//                     ValidIssuer = issuer,
//                     ValidAudience = audience,
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
//                 };
//             });
//         }
//     }

// }