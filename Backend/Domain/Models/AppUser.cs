using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class AppUser
    {
        public Guid UserId { get; set; }

        [Required]
        public string Role { get; set; } = null!;
    }
}
