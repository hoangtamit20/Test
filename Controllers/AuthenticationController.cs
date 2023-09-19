using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetShop.Configurations;
using PetShop.Entity;
using PetShop.Models;
using PetShop.Services;

namespace PetShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<NguoiDung> _userManager;
        // private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<NguoiDung> userManager, IConfiguration configuration) => (_userManager, _configuration) = (userManager, configuration);

        [HttpPost("/dang-ky")]
        public async Task<IActionResult> DangKy([FromBody] NguoiDungRegisterModel nguoiDungRegisterModel)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung_Exist = await _userManager.FindByEmailAsync(nguoiDungRegisterModel.Email!);
                if (nguoiDung_Exist == null)
                {
                    var nguoiDung = new NguoiDung(){
                        Email = nguoiDungRegisterModel.Email!,
                        UserName = nguoiDungRegisterModel.Email!,
                    };
                    var isCreate = await _userManager.CreateAsync(nguoiDung, nguoiDungRegisterModel.Password!);
                    if (isCreate.Succeeded)
                    {
                        // generate token
                        var token = JwtToken.GenerateJwtToken(nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                        return Ok(new JwtResponseModel(){
                           Result = true,
                           Token = token,
                           Errors = null 
                        });
                    }
                    return BadRequest(new JwtResponseModel(){
                        Errors = new List<string>(){
                            "Lỗi hệ thống!"   
                        },
                        Result = false
                    });
                }
                return BadRequest(new JwtResponseModel(){
                    Errors = new List<string>(){
                        "Email đã được sử dụng!"
                    }
                });
            }
            return BadRequest();
        }

        [HttpPost("/dang-nhap")]
        public async Task<IActionResult> DangNhap([FromBody] NguoiDungDangNhap nguoiDungDangNhap)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _userManager.FindByEmailAsync(nguoiDungDangNhap.UserName!);
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
                var isCorrect = await _userManager.CheckPasswordAsync(nguoiDung, nguoiDungDangNhap.Password!);
                if (!isCorrect)
                    return BadRequest(new JwtResponseModel(){
                        Result = false,
                        Errors = new List<string>(){
                            "Mật khẩu không đúng!"
                        }
                    });
                var jwtToken = JwtToken.GenerateJwtToken(nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                return Ok(new JwtResponseModel(){
                    Result = true,
                    Token = jwtToken
                });
            }
            return BadRequest(new JwtResponseModel(){
                Result = false,
                Errors = new List<string>(){
                    "Tên đăng nhập hoặc mật khẩu không hợp lệ"
                }
            });
        }
    }
}