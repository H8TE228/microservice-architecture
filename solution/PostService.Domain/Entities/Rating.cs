using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostService.Domain.Entities
{
    public enum RatingType
    {
        Like,
        Dislike
    }

    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid RaterUserId { get; set; }

        [Required]
        public Guid RatedUserId { get; set; }

        [Required]
        public RatingType Type { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;
    }
}