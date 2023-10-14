using System.Net;
using System.Web;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PetShop.Models;
using PetShop.Services;
using serverapi.Base;
using serverapi.Dtos.Auths;
using serverapi.Entity;
using serverapi.Services;
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

        private readonly IEmailSender _emailSender;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="configuration"></param>
        /// <param name="googleService"></param>
        /// <param name="emailSender"></param>
        public AuthenticationController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IGoogleService googleService, IEmailSender emailSender) => (_userManager, _roleManager, _configuration, _googleService, _emailSender) = (userManager, roleManager, configuration, googleService, emailSender);


        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="nguoiDungRegisterModel"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST:
        /// {
        ///     "name":"Võ Văn A",
        ///     "email":"vovana@gmail.com",
        ///     "password":"VoVanA=123"
        /// }
        /// </remarks>
        [HttpPost("/dang-ky")]
        [ProducesResponseType(typeof(JwtResponseModel<UserDataDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
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
                        return Ok(new JwtResponseModel<UserDataDto>()
                        {
                            Result = true,
                            Token = token.AccessToken!,
                            RefreshToken = token.RefreshToken!,
                            Data = nguoiDung.Adapt<UserDataDto>()
                        });
                    }
                    return BadRequest(new BaseBadRequestResult()
                    {
                        Errors = new List<string>(){
                            "Lỗi hệ thống!"
                        },
                    });
                }
                return BadRequest(new BaseBadRequestResult()
                {
                    Errors = new List<string>(){
                        "Email đã được sử dụng!"
                    }
                });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginResponse"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST:
        /// {
        ///     "email":"vovana@gmail.com",
        ///     "password":"VoVanA=123"
        /// }
        /// </remarks>

        [HttpPost("/dang-nhap")]
        [ProducesResponseType(typeof(JwtResponseModel<UserDataDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DangNhap([FromBody] LoginResponse loginResponse)
        {
            if (ModelState.IsValid)
            {
                var nguoiDung = await _userManager.FindByEmailAsync(loginResponse.UserName!);
                if (nguoiDung is null)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { "Tên đăng nhập không tồn tại!" } });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(nguoiDung, loginResponse.Password!);
                if (!isCorrect)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { "Mật khẩu không đúng!" } });
                var jwtToken = await JwtToken.GenerateJwtToken(_userManager, nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                return Ok(new JwtResponseModel<UserDataDto>()
                {
                    Result = true,
                    Token = jwtToken.AccessToken!,
                    RefreshToken = jwtToken.RefreshToken!,
                    Data = nguoiDung.Adapt<UserDataDto>()
                });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }


        /// <summary>
        /// Api login with google
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST: google-login
        /// {
        ///     "token" : ""
        /// }
        /// </remarks>
        [HttpPost("/google-login")]
        [ProducesResponseType(typeof(JwtResponseModel<UserDataDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromHeader] string token)
        {
            // Gọi hàm GetUserInfoAsync của GoogleService để lấy thông tin người dùng từ Google
            var userInfo = await _googleService.GetUserInfoAsync(token);

            // Tạo một đối tượng NguoiDung từ thông tin người dùng
            var nguoiDung = new AppUser
            {
                Name = userInfo.Name!,
                Email = userInfo.Email,
                ImageUrl = userInfo.Picture
            };

            var nd = await _userManager.FindByEmailAsync(nguoiDung.Email!);
            if (nd is null)
            {
                nguoiDung.UserName = userInfo.Email;
                var isCreate = await _userManager.CreateAsync(nguoiDung);
                if (isCreate.Succeeded)
                {
                    // Update the user with the additional properties
                    // await _userManager.UpdateAsync(nguoiDung);
                    var user = await _userManager.FindByEmailAsync(nguoiDung.Email!);
                    if (user is not null)
                    {
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    // generate token
                    var tokenCreate = await JwtToken.GenerateJwtToken(_userManager, nguoiDung, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                    return Ok(new JwtResponseModel<UserDataDto>()
                    {
                        Result = true,
                        Data = nguoiDung.Adapt<UserDataDto>(),
                        Token = tokenCreate.AccessToken!,
                        RefreshToken = tokenCreate.RefreshToken!
                    });
                }
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { "Create user failed!" } });
            }
            else
            {
                // Gọi hàm GenerateJwtToken để tạo ra jwt token từ đối tượng NguoiDung
                var jwtToken = await JwtToken.GenerateJwtToken(_userManager, nd, _configuration.GetSection("JwtConfig:SecretKey").Value!);
                if (nd.ImageUrl is null)
                {
                    nd.ImageUrl = nguoiDung.ImageUrl;
                }
                await _userManager.UpdateAsync(nd);

                // Trả về jwt token cho client
                return Ok(new JwtResponseModel<UserDataDto>()
                {
                    Result = true,
                    Data = nd.Adapt<UserDataDto>(),
                    Token = jwtToken.AccessToken!,
                    RefreshToken = jwtToken.RefreshToken!
                });
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST:
        /// {
        ///     "email":"hoangtamit20@gmail.com",
        /// }
        /// </remarks>
        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(typeof(BaseResultSuccess), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(email);
                // check user exists
                if (userExist is not null)
                {
                    if (!(await _userManager.IsEmailConfirmedAsync(userExist)))
                    {
                        userExist.EmailConfirmed = true;
                        var result = await _userManager.UpdateAsync(userExist);

                        if (result.Succeeded)
                        {
                            // generate token
                            var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(userExist);
                            // process send email service
                            var uriBuilder = new UriBuilder("http://localhost:3000/update-password");
                            var parameters = HttpUtility.ParseQueryString(string.Empty);
                            parameters["id"] = userExist.Id.ToString();
                            parameters["token"] = tokenReset;
                            uriBuilder.Query = parameters.ToString();
                            Uri finalUrl = uriBuilder.Uri;
                            await _emailSender.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking here: <a href='{finalUrl}'>link</a>");
                            return Ok(new BaseResultSuccess() { Message = $"Please confirm email to change password!" });
                        }
                        else
                        {
                            return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { "Error updating user" } });
                        }
                    }
                    else
                    {
                        // generate token
                        var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(userExist);
                        // process send email service
                        var uriBuilder = new UriBuilder("http://localhost:3000/update-password");
                        var parameters = HttpUtility.ParseQueryString(string.Empty);
                        parameters["id"] = userExist.Id.ToString();
                        parameters["token"] = tokenReset;
                        uriBuilder.Query = parameters.ToString();
                        Uri finalUrl = uriBuilder.Uri;
                        await _emailSender.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking here: <a href='{finalUrl}'>link</a>");
                        return Ok(new BaseResultSuccess() { Message = $"Please confirm email to change password!" });
                    }

                }
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Email : {email} not exists" } });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("decode-token-resetpassword")]
        public IActionResult DecodeToken(string token)
        {
            try
            {
                return Ok(new BaseResultWithData<string>()
                {
                    Success = true,
                    Message = "Decode token success!",
                    Data = Uri.UnescapeDataString(token)
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"{ex.Message}"}});
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetPasswordConfirmRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset-password-confirm")]
        public async Task<IActionResult> ResetPasswordConfirm([FromBody] ResetPasswordConfirmRequestDto resetPasswordConfirmRequestDto)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByIdAsync(resetPasswordConfirmRequestDto.UserId);
                // check user exists
                if (userExist is not null)
                {
                    var resetPassResult = await _userManager.ResetPasswordAsync(userExist, resetPasswordConfirmRequestDto.Token, resetPasswordConfirmRequestDto.NewPassword);
                    if (resetPassResult.Succeeded)
                    {
                        return Ok(new BaseResultSuccess() { Message = "Change password successed!" });
                    }
                    else
                    {
                        return BadRequest(new BaseBadRequestResult() { Errors = resetPassResult.Errors.Select(e => e.Description).ToList() });
                    }
                }
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"User Id : {resetPasswordConfirmRequestDto.UserId} not exists" } });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
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