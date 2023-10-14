using System.ComponentModel.DataAnnotations;

namespace serverapi.Dtos.Auths
{
    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordRequestDto
    {
        /// <summary>
        /// 
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public required string ReNewPassword { get; set; }
    }
}