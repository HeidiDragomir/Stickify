namespace Backend.Domain.Models
{
    public class Collection
    {
        public Guid CollectionId { get; set; }

        public Guid UserId { get; }

        public Guid StickýNoteId { get; }
    }
}
