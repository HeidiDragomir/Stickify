namespace Backend.Domain.DTOs.Auth
{
    public class AuthResDTO
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime ExpireAt { get; set; }

        public ICollection<string> Roles { get; set; } = new List<string>();
        public ICollection<CollectionDTO> Collections { get; set; } = new List<CollectionDTO>();


    }
}
