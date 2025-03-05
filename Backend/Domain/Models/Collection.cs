using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class Collection
    {
        public Guid CollectionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        public Guid UserId { get; } // Foreign Key

        public AppUser? User { get; set; } // Navigation Property

        public ICollection<StickyNote> StickyNotes { get; set; } = new List<StickyNote>();
    }
}
