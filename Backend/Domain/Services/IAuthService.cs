using Backend.Domain.DTOs.Auth;

namespace Backend.Domain.Services
{
    public interface IAuthService
    {

        Task<AuthResDTO?> Register(RegisterDTO dto);

        Task<AuthResDTO?> Login(LoginDTO dto);

        Task<AuthResDTO?> RefreshToken(string token);
    }
}
