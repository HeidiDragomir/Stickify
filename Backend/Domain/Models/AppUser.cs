using Microsoft.AspNetCore.Identity;

namespace Backend.Domain.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();
    }
}
