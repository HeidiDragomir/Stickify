using AutoMapper;
using Backend.Domain.DTOs.Auth;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Backend.Utils;
using Humanizer;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services
{

    // <summary>
    // Handles user authentication and registration logic.
    // Uses ASP.NET Core Identity to manage users, roles and passwords.
    // Generates JWT tokens for frontend app.
    // </summary>
    public class AuthService : IAuthService
    {

        private readonly UserManager<AppUser> _userManager;  // Helps create, update, find and manage users
        private readonly IJwtTokenManager _jwtTokenManager;
        private readonly IMapper _mapper;

        public AuthService(UserManager<AppUser> userManager, IJwtTokenManager jwtTokenManager, IMapper mapper)
        {
            _userManager = userManager;
            _jwtTokenManager = jwtTokenManager;
            _mapper = mapper;
        }


        /// <summary>
        /// Registers a new user.
        /// Steps:
        /// 1. Check if email already exists.
        /// 2. Create AppUser object with CreatedAt timestamp.
        /// 3. Use UserManager to create the user in DB.
        /// 4. Assign default role ("User").
        /// 5. Generate JWT for immediate authentication.
        /// 6. Map AppUser to AuthResponseDto using AutoMapper.
        /// </summary>
        public async Task<AuthResDTO?> Register(RegisterDTO dto)
        {
            // 1. Check if user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);


            //if (existingUser != null) return null;

            // 2. Create new AppUser object
            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,    // capture the moment user is created
            };


            // 3. Create user in database
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return null;

            // 4. Asign default role
            await _userManager.AddToRoleAsync(user, "User");

            // 5. Generate JWT Token
            var token = await _jwtTokenManager.GenerateToken(user, _userManager);

            // 6. Map AppUser to AuthResDTO
            var authResponse = _mapper.Map<AuthResDTO>(user);

            authResponse.UserId = user.Id;
            authResponse.Token = token;
            authResponse.ExpireAt = DateTime.UtcNow.AddMinutes(60); // token lifetime
            authResponse.Role = "User"; // set role explicitly


            return authResponse;

        }


        /// <summary>
        /// Logs in a user using email and password.
        /// Steps:
        /// 1. Check if a user with the given email exists.
        /// 2. Verify the provided password is correct.
        /// 3. Fetch the user's roles.
        /// 4. Generate a JWT token for authentication.
        /// 5. Map the user to AuthResDto using AutoMapper.
        /// 6. Return the AuthResDto with token, expiration and role.
        /// </summary>
        public async Task<AuthResDTO?> Login(LoginDTO dto)
        {
            // 1. Check if a user with the given email exists.
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null) return null;

            // 2. Verify is the provided password is correct
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid) return null;

            // 3. Fetch the user's roles.
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            // 4. Generate a JWT token for authentication.
            var token = await _jwtTokenManager.GenerateToken(user, _userManager);

            // 5.Map the user to AuthResDto;
            var authResponse = _mapper.Map<AuthResDTO>(user);

            // 6. Return the AuthResDto with token, expiration and role.
            authResponse.UserId = user.Id;
            authResponse.Token = token;
            authResponse.ExpireAt = DateTime.UtcNow.AddMinutes(60);
            authResponse.Role = role;


            return authResponse;
        }


        /// <summary>
        /// Refreshes an expired or near-expired token.
        /// Keeps the same user identity but generates a new token.
        /// </summary>
        public async Task<AuthResDTO?> RefreshToken(string token)
        {
            // Generate new token from old one
            var newToken = await _jwtTokenManager.RefreshToken(token, _userManager);

            if (newToken == null) return null;

            // Extract user info from old token
            var principal = _jwtTokenManager.GetPrincipalFromToken(token);

            var userId = principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return null;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var authResponse = _mapper.Map<AuthResDTO>(user);

            authResponse.Token = newToken;
            authResponse.ExpireAt = DateTime.UtcNow.AddMinutes(60);
            authResponse.Role = role;

            return authResponse;
        }
    }

}
