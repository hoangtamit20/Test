using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PetShop.Configurations;
using PetShop.Entity;

namespace PetShop.Services
{
    public static class JwtToken
    {
        public static string GenerateJwtToken(NguoiDung nguoiDung, string secretKey)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim("Id", nguoiDung.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email!),
                    new Claim("ImageUrl", nguoiDung.ImageUrl!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}