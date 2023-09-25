using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PetShop.Configurations;
using PetShop.Entity;
using serverapi.Models;

namespace PetShop.Services
{
    public static class JwtToken
    {

        public static string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }

        public static async Task<TokenModel> GenerateJwtToken(UserManager<NguoiDung> userManager, NguoiDung nguoiDung, string secretKey)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var listClaim = new List<Claim>()
            {
                new Claim("Id", nguoiDung.Id),
                new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email!),
                new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            };

            foreach (var role in await userManager.GetRolesAsync(nguoiDung))
            {
                listClaim.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(listClaim),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return new TokenModel(){
                AccessToken = jwtToken,
                RefreshToken = GenerateRefreshToken()
            };
        }
    }
}