using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostService.Domain.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string UserFirstName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string UserLastName { get; set; } = string.Empty;
        public int UserAge { get; set; }
        [MaxLength(500)]
        public string UserMainPhotoUrl { get; set; } = string.Empty;
        
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}