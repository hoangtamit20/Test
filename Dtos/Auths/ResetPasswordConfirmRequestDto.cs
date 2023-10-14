namespace serverapi.Dtos.Auths
{
    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordConfirmRequestDto
    {
        /// <summary>
        /// 
        /// </summary>
        public required string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public required string Token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public required string NewPassword { get; set; }
    }
}