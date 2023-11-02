// using Microsoft.Extensions.Configuration;
// using System.Text;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerceApp.Errors;
using ECommerceApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceApp.Services
{
    public class TokenObject
    {
        public string ? DecodedToken { get; set; }
        public string ? DecodedTokenAudience { get; set; }
    } 
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public TokenService(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }


        public string GenerateToken(string email)
        {
            var JwtKey = _configuration["Jwt:Key"];
            if (JwtKey == null)
                return "";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtKey);
            // var key = new HMACSHA512(Convert.FromBase64String(JwtKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public TokenObject GetUserFromToken(string token)
        {
            var JwtKey = _configuration["Jwt:Issuer"] ?? throw new IsNullException();
            
            var decodedToken = new JwtSecurityToken(jwtEncodedString: token)
                .Payload
                .First(x => x.Key == "email")
                .Value
                .ToString();

            var decodedTokenAudience = new JwtSecurityToken(jwtEncodedString: token)
                .Payload
                .First(x => x.Key == "aud")
                .Value
                .ToString();

            var logger = _loggerFactory.CreateLogger<TokenService>();
            logger?.LogInformation("Decoded token", decodedTokenAudience);
            
            return new TokenObject(){
                DecodedToken = decodedToken,
                DecodedTokenAudience = decodedTokenAudience
            };

            // var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(JwtKey);
            // var securityKey = new SymmetricSecurityKey(key);
            // var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // var tokenValidationParameters = new TokenValidationParameters
            // {
            //     ValidateIssuerSigningKey = true,
            //     IssuerSigningKey = signingCredentials.Key,
            //     ValidateIssuer = true,
            //     ValidIssuer = _configuration["Jwt:Issuer"],
            //     ValidateAudience = true,
            //     ValidAudience = _configuration["Jwt:Audience"],
            //     ValidateLifetime = true,
            //     ClockSkew = TimeSpan.Zero
            // };

            // try
            // {
            //     tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            //     var jwtToken = (JwtSecurityToken)validatedToken;
            //     var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            //     return email;
            // }
            // catch (Exception exceptio
            // {
            //     var logger = _loggerFactory.CreateLogger<TokenService>();
            //     logger?.LogError(exception, "An error occurred while verifying token");
            //     throw;
            // }
        }
    }
}