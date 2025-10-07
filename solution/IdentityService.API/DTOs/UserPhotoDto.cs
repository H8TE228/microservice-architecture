using System;

namespace IdentityService.API.DTOs
{
    public class UserPhotoDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsMain { get; set; }
        public DateTime UploadDate { get; set; }
    }
}