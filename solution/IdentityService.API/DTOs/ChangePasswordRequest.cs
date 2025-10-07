using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long")]
        public string NewPassword { get; set; } = string.Empty;
    }
}