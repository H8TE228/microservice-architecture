using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class LogoutRequest
    {
        [Required(ErrorMessage = "Refresh Token is required for logout")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}