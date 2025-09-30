using Backend.Domain.Models;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Utils
{

    // Interface to define the contract for JWT operations.
    // This makes the code easier to test and allows to add implementations later if needed.
    public interface IJwtTokenManager
    {
        // async because we fetch roles
        Task<string> GenerateToken(AppUser user, UserManager<AppUser> userManager); // Creates a new token
        bool ValidateToken(string token);   // Checks if a token is valid
        ClaimsPrincipal? GetPrincipalFromToken(string token);   // Extracts the claims (user info) from a token

        // async because we fetch user
        Task<string?> RefreshToken(string token, UserManager<AppUser> userManager); // Generates a new token using an old one
    }


    public class JwtTokenManager : IJwtTokenManager
    {
        // Stores configuration values from appsettings.json (secret key, ...)
        private readonly IConfiguration _configuration;

        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        // Injects configuration settings via dependency injection
        public JwtTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;

            // SecretKey is mandatory --> without it tokens cannot be signed securely
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new ArgumentNullException("JwtSecretKey is missing.");

            // Issuer is mandatory --> ensures tokens were issued by system
            _issuer = _configuration.GetValue<string>("Jwt:Issuer")
                ?? throw new ArgumentNullException("Jwt:Issuer is missing.");

            // Audience is mandatory --> ensures tokens are meant for API
            _audience = _configuration.GetValue<string>("Jwt:Audience")
                ?? throw new ArgumentNullException("Jwt:Audience is missing.");

            // Expiration can fall back to 60 minutes if not configured
            _expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
        }

        // This method creates a JWT token for a user with their claims (id, email, role, etc.)
        public async Task<string> GenerateToken(AppUser user, UserManager<AppUser> userManager)
        {
            // Creates a new JWT token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Converts secret key to byte array for encryption
            var key = Encoding.UTF8.GetBytes(_secretKey);

            // Fetch roles from database
            var roles = await userManager.GetRolesAsync(user);
            var roleClaim = roles.FirstOrDefault() ?? Roles.User; // take first role or default

            // Claims are just pieces of information about the user (like role, email, etc.)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Role, roleClaim),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Iat,
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64) // Issued at
            };

            // Token description (who issued it, who can use it, when it expires, etc.)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),   // Sets token expiration to 60 minutes from current time
                Issuer = _issuer,   // Sets the token issuer
                Audience = _audience,   // Sets the intended audience
                SigningCredentials = new SigningCredentials(    // Defines how the token is signed
                                     new SymmetricSecurityKey(key),  // Uses the secret key for signing
                                     SecurityAlgorithms.HmacSha256Signature) // Algorithm, uses HMAC SHA-256 encryption
            };

            // Generates the JWT token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Converts the token to a string and returns it
            return tokenHandler.WriteToken(token);
        }


        // Checks if a token is valid and signed with the correct key
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            // Creates a new JWT token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Converts secret key to byte array for encryption
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true, // Ensures token is not expired
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false; // Token is invalid
            }
        }


        // Extracts the user info (claims) from the token
        // This ignores expiration check --> useful for Refresh Tokens
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            // Creates a new JWT token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Converts secret key to byte array for encryption
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = false, // Lifetime is ignored --> allows expired tokens for refresh
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }


        // Generates a new token using data from an existing one
        public async Task<string?> RefreshToken(string token, UserManager<AppUser> userManager)
        {
            // Extract claims from the old token
            var principal = GetPrincipalFromToken(token);

            if (principal == null)
                return null;

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return null;

            // Fetch user from database
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            // Generate a new token for this user (roles are fetched inside GenerateToken)
            return await GenerateToken(user, userManager);
        }

    }
}
