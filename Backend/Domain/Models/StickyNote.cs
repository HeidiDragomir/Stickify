using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class StickyNote
    {
        public Guid StickyNoteId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public Guid CollectionId { get; } // Foreign key

        public Collection? Collection { get; set; } // Navigation Property

    }
}
