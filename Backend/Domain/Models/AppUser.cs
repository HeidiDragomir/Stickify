using Microsoft.AspNetCore.Identity;

namespace Backend.Domain.Models
{
    public class AppUser : IdentityUser
    {
        // Each user can have multiple collections
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

        // Record when the user was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
