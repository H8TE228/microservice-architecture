using System;
using System.Collections.Generic;

namespace IdentityService.API.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? ZodiacSign { get; set; }
        public string? City { get; set; }
        public string? Education { get; set; }
        public string? Interests { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserPhotoDto> Photos { get; set; } = new List<UserPhotoDto>();
    }
}