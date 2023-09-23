using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetShop.Configurations;
using PetShop.Entity;
using PetShop.Models;
using PetShop.Services;
using serverapi.Models;
using serverapi.Services.Iservice;

namespace PetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;

        private readonly IGoogleService _googleService;
        // private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<NguoiDung> userManager, SignInManager<NguoiDung> signInManager, IConfiguration configuration, IGoogleService googleService) => (_userManager, _signInManager, _configuration, _googleService) = (userManager, signInManager, configuration, googleService);

        [HttpPost("/dang-ky")]
        public async Task<IActionResult> DangKy([FromBody] NguoiDungRegisterModel nguoiDungRegisterModel)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung_Exist = await _userManager.FindByEmailAsync(nguoiDungRegisterModel.Email!);
                if (nguoiDung_Exist == null)
                {
                    var nguoiDung = new NguoiDung()
                    {
                        Email = nguoiDungRegisterModel.Email!,
                        UserName = nguoiDungRegisterModel.Email!,
                    };
                    var isCreate = await _userManager.CreateAsync(nguoiDung, nguoiDungRegisterModel.Password!);
                    if (isCreate.Succeeded)
                    {
                        // generate token
                        var token = JwtToken.GenerateJwtToken(nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                        return Ok(new JwtResponseModel()
                        {
                            Result = true,
                            Token = token,
                            Errors = null
                        });
                    }
                    return BadRequest(new JwtResponseModel()
                    {
                        Errors = new List<string>(){
                            "Lỗi hệ thống!"
                        },
                        Result = false
                    });
                }
                return BadRequest(new JwtResponseModel()
                {
                    Errors = new List<string>(){
                        "Email đã được sử dụng!"
                    }
                });
            }
            return BadRequest();
        }

        [HttpPost("/dang-nhap")]
        public async Task<IActionResult> DangNhap([FromBody] LoginResponse loginResponse)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _userManager.FindByEmailAsync(loginResponse.UserName!);
                if (nguoiDung is null)
                {
                    return BadRequest(new JwtResponseModel()
                    {
                        Result = false,
                        Errors = new List<string>(){
                            "Tên đăng nhập không tồn tại!"
                        }
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(nguoiDung, loginResponse.Password!);
                if (!isCorrect)
                    return BadRequest(new JwtResponseModel()
                    {
                        Result = false,
                        Errors = new List<string>(){
                            "Mật khẩu không đúng!"
                        }
                    });
                var jwtToken = JwtToken.GenerateJwtToken(nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);

                return Ok(new JwtResponseModel()
                {
                    Result = true,
                    Token = jwtToken,
                    Data = new LoginRequest(){
                        Name = nguoiDung.Name,
                        Email = nguoiDung.Email,
                        ImageUrl = nguoiDung.ImageUrl
                    }
                });
            }
            return BadRequest(new JwtResponseModel()
            {
                Result = false,
                Errors = new List<string>(){
                    "Tên đăng nhập hoặc mật khẩu không hợp lệ"
                }
            });
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromHeader] string token)
        {
            // Gọi hàm GetUserInfoAsync của GoogleService để lấy thông tin người dùng từ Google
            var userInfo = await _googleService.GetUserInfoAsync(token);

            // Tạo một đối tượng NguoiDung từ thông tin người dùng
            var nguoiDung = new NguoiDung
            {
                Id = userInfo.Email!,
                Email = userInfo.Email,
                Name = userInfo.Name,
                ImageUrl = userInfo.ImageUrl
            };

            // Gọi hàm GenerateJwtToken để tạo ra jwt token từ đối tượng NguoiDung
            var jwtToken = JwtToken.GenerateJwtToken(nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);

            // Trả về jwt token cho client
            return Ok(new JwtResponseModel(){
                Result = true,
                Data = userInfo,
                Token = jwtToken
            });
        }
    }
}