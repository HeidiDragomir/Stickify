using Backend.Domain.DTOs.Auth;

namespace Backend.Domain.Services
{
    public interface IAuthService
    {

        Task<RegisterResDTO> Register(RegisterReqDTO user);

        Task<AuthResDTO> Login(AuthReqDTO user);
    }
}
