using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Infrastructure.Entities
{
    public class UserPhoto 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public int Order { get; set; }

        public bool IsMain { get; set; } = false;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}