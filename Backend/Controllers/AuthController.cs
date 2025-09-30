using Backend.Domain.DTOs.Auth;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    /// <summary>
    /// Controller responsible for handling authentication endpoints (register, login, refresh).
    /// This is what the frontend  will call when dealing with user accounts.
    /// The controller itself does not contain business logic,
    /// it simply delegates the work to AuthService.
    /// </summary>

    [ApiController] // Makes model validation automatic (checks DTOs against [Required], etc.)
    [Route("api/[controller]")] // URL will be: api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inject the AuthService
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        /// <summary>
        /// Register a new user.
        /// 1. Receives RegisterDTO from frontend.
        /// 2. Calls AuthService.Register.
        /// 3. Returns AuthResDTO with token, role, expiration if successful.
        /// </summary>

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // automatic validation check

            var result = await _authService.Register(dto);

            if (result == null)
                return BadRequest("Registration failed. Email may already be in use.");

            return Ok(result);  // returns 200 OK with AuthResDTO (token + user info)
        }


        /// <summary>
        /// Logs in a user.
        /// 1. Receives LoginDTO from frontend.
        /// 2. Calls AuthService.Login.
        /// 3. Returns AuthResDTO with token, role, expiration if successful.
        /// </summary>

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(dto);

            if (result == null)
                return Unauthorized("Invalid email or password."); // 401 response

            return Ok(result);
        }


        /// <summary>
        /// Refreshes a JWT token when it's about to expire.
        /// 1. Receives the old token from frontend.
        /// 2. Calls AuthService.RefreshToken.
        /// 3. Returns a new AuthResDTO with a fresh token.
        /// </summary>

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required.");

            var result = await _authService.RefreshToken(token);

            if (result == null)
                return Unauthorized("Invalid or expired token.");

            return Ok(result);

        }

    }
}
