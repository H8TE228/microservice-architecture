using System.ComponentModel.DataAnnotations;

namespace IdentityService.API.DTOs
{
    public class DeleteAccountRequest
    {
        [Required(ErrorMessage = "Password confirmation is required")]
        public string PasswordConfirmation { get; set; } = string.Empty;
    }
}