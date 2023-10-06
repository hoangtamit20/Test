using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetShop.Models;
using PetShop.Services;
using serverapi.Base;
using serverapi.Dtos.Auths;
using serverapi.Entity;
using serverapi.Services.Iservice;

namespace PetShop.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        // private readonly SignInManager<NguoiDung> _signInManager;

        private readonly IGoogleService _googleService;
        // private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="configuration"></param>
        /// <param name="googleService"></param>
        public AuthenticationController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IGoogleService googleService) => (_userManager, _roleManager, _configuration, _googleService) = (userManager, roleManager, configuration, googleService);


        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="nguoiDungRegisterModel"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST :
        /// {
        ///     "Name" = "Võ Văn A",
        ///     "Email" = "vovana@gmail.com",
        ///     "Password" = "VoVanA=123"
        /// }
        /// </remarks>
        [HttpPost("/dang-ky")]
        public async Task<IActionResult> DangKy([FromBody] NguoiDungRegisterModel nguoiDungRegisterModel)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung_Exist = await _userManager.FindByEmailAsync(nguoiDungRegisterModel.Email!);
                if (nguoiDung_Exist == null)
                {
                    var nguoiDung = new AppUser()
                    {
                        CreateDate = DateTime.Now,
                        Email = nguoiDungRegisterModel.Email!,
                        UserName = nguoiDungRegisterModel.Email!,
                        Name = nguoiDungRegisterModel.Name!
                    };
                    var isCreate = await _userManager.CreateAsync(nguoiDung, nguoiDungRegisterModel.Password!);
                    if (isCreate.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(nguoiDung.Email);
                        if (user is not null)
                        {
                            if (!await _roleManager.RoleExistsAsync("User"))
                            {
                                await _roleManager.CreateAsync(new IdentityRole("User"));
                            }
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                        // generate token
                        var token = await JwtToken.GenerateJwtToken(_userManager, nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                        return Ok(new JwtResponseModel()
                        {
                            Result = true,
                            Token = token.AccessToken!,
                            RefreshToken = token.RefreshToken!
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

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginResponse"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST
        /// {
        ///     Email = "vovana@gmail.com",
        ///     Password = "VoVanA=123"
        /// }
        /// </remarks>

        [HttpPost("/dang-nhap")]
        [ProducesResponseType(typeof(BaseLoginResultWithData<UserDataDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResultBadRequest), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DangNhap([FromBody] LoginResponse loginResponse)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _userManager.FindByEmailAsync(loginResponse.UserName!);
                if (nguoiDung is null)
                {
                    return BadRequest(new BaseResultBadRequest()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Tên đăng nhập không tồn tại!"
                        }
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(nguoiDung, loginResponse.Password!);
                if (!isCorrect)
                    return BadRequest(new BaseResultBadRequest()
                    {
                        Success = false,
                        Errors = new List<string>(){
                            "Mật khẩu không đúng!"
                        }
                    });
                var jwtToken = await JwtToken.GenerateJwtToken(_userManager, nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);

                return Ok(new BaseLoginResultWithData<UserDataDto>()
                {
                    Result = true,
                    Token = jwtToken.AccessToken!,
                    RefreshToken = jwtToken.RefreshToken!,
                    Data = nguoiDung.Adapt<UserDataDto>()
                });
            }
            return BadRequest(new BaseLoginResultWithData<UserDataDto>()
            {
                Result = false,
                Errors = new List<BaseError>(){
                    new BaseError()
                    {
                        Code = "",
                        Message = "Tên đăng nhập hoặc mật khẩu không hợp lệ"
                    }
                }
            });
        }


        /// <summary>
        /// Api login with google
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST : google-login
        /// {
        ///     "token" : ""
        /// }
        /// </remarks>
        [HttpPost("/google-login")]
        public async Task<IActionResult> GoogleLogin([FromHeader] string token)
        {
            // Gọi hàm GetUserInfoAsync của GoogleService để lấy thông tin người dùng từ Google
            var userInfo = await _googleService.GetUserInfoAsync(token);

            // Tạo một đối tượng NguoiDung từ thông tin người dùng
            var nguoiDung = new AppUser
            {
                Id = userInfo.Email!,
                Email = userInfo.Email,
                Name = userInfo.Name!,
                ImageUrl = userInfo.ImageUrl
            };

            // Gọi hàm GenerateJwtToken để tạo ra jwt token từ đối tượng NguoiDung
            var jwtToken = await JwtToken.GenerateJwtToken(_userManager, nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);

            // Trả về jwt token cho client
            return Ok(new BaseLoginResultWithData<UserDataDto>()
            {
                Result = true,
                Data = userInfo.Adapt<UserDataDto>(),
                Token = jwtToken.AccessToken!,
                RefreshToken = jwtToken.RefreshToken!
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/TestAuth")]
        [Authorize(Roles = "Admin")]
        public string GetLetMe()
        {
            return "aldhalihdlaidhwli";
        }
    }
}