using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class Collection
    {
        public Guid CollectionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set;  } // Foreign Key

        public AppUser? User { get; set; } // Navigation Property

        public ICollection<StickyNote> StickyNotes { get; set; } = new List<StickyNote>();
    }
}
