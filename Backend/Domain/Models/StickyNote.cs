using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class StickyNote
    {
        public Guid StickyNoteId { get; set; }

        public Guid UserId { get; }

        public Guid CollectionId { get; }

        [Required]
        public string BodyText { get; set; } = null!;
    }
}
