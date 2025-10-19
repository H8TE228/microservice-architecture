using System;

namespace PostService.Application.DTOs
{
    public class IdentityUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string MainPhotoUrl { get; set; } = string.Empty;
    }
}