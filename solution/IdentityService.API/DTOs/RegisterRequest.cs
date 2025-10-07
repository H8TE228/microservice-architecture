using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FirstName { get; set; } // Опционально

        [MaxLength(50)]
        public string? LastName { get; set; } // Опционально
    }
}