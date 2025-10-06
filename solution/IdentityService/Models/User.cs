using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models
{
    public class User
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        [MaxLength(50)]
        public string ZodiacSign { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(256)]
        public string Education { get; set; }
        
        [MaxLength(1000)]
        public string Interests { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }

        public bool IsActive { get; set; } = true;
        
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}