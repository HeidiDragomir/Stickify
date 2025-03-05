using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Models
{
    public class AppUser
    {
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

        //[Required]
        //public string Role { get; set; } = null!;
    }
}
