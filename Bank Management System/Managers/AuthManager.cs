using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bank_Management_System.DTO;
using Bank_Management_System.Interfaces;
using Bank_Management_System.models;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bank_Management_System.Managers
{
    public class AuthManager : IAuthService
    {
        private readonly BankDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<string> _passwordHasher;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(AuthManager));

        public AuthManager(BankDbContext context, IConfiguration config) {
            _dbContext = context;
            _configuration = config;
            _passwordHasher = new PasswordHasher<string>();
        }

        public async Task<RegisterResponseDto> RegisterAsync(UserRegisterModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                    return new RegisterResponseDto { Message = "Invalid input: Email and Password are required.", User = null };

                if (model.Password.Length < 6)
                    return new RegisterResponseDto { Message = "Password must be at least 6 characters long.", User = null };

                var user = await _dbContext.Users.AnyAsync(u => u.Email == model.Email);
                if (user) return new RegisterResponseDto { Message = "User already exists!", User = null };

                var hashedPassword = _passwordHasher.HashPassword(null, model.Password);

                var newUser = new User
                {
                    UserId = Guid.NewGuid(),
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = hashedPassword,
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow,
                    Status = "Active"
                };

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                _logger.Info($"New user registered: {model.Email}");
                return new RegisterResponseDto { Message = "User registered successfully!", User = newUser };
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during user registration for {model.Email}");
                return new RegisterResponseDto { Message = "An error occurred while registering the user.", User = null };
            }
        }

        public async Task<(string Token, object User)> LoginAsync(UserLoginModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                    return (null,"Invalid input: Email and Password are required.");

                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return (null, "User not found!");
                }

                var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    return (null, "Invalid credentials");
                }

                _logger.Info($"User logged in: {model.Email}");
                
                var token = GenerateJwtToken(user);

                var userDto = new
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Role = user.Role
                };

                return (token, userDto);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during login for {model.Email}");
                return (null, "An error occurred while processing your login.");
            }
        }

        public Task<bool> LogoutAsync()
        {
            try
            {
                _logger.Info("User logged out successfully.");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred during logout.");
                return Task.FromResult(false);
            }
        }

        public async Task<UserDto> GetAuthUserAsync(string token) {
            try
            {
                var validUser = ValidateJwtToken(token);

                if (validUser == null)
                {
                    _logger.Error($"Invalid JWT token: {token}");
                    return null;
                }

                var userIdClaim = validUser.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.Error("JWT token does not contain user identifier");
                    return null;
                }

                if(!Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    _logger.Error("Invalid user ID format in token.");
                    return null;
                }

                var user = await _dbContext.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role,
                        Status = u.Status
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.Error($"User not found for token with UserID: {userId}");
                    return null;
                }

                return user;
            }
            catch (Exception ex) {
                _logger.Error("Error retrieving authenticated user.");
                return null;
            }
        }
       
        private ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var secretKey = _configuration["JwtSettings:SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    _logger.Error("JWT Secret Key is missing in configuration.");
                    return null;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    return principal;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("JWT validation failed.");
                return null;
            }
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var secretKey = _configuration["JwtSettings:SecretKey"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    _logger.Error("JWT Secret Key, Issuer, or Audience is missing in configuration.");
                    throw new InvalidOperationException("JWT settings are not properly configured.");
                }

                var key = Encoding.UTF8.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = issuer,  
                    Audience = audience,  
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while generating JWT token for user {user.Email}");
                throw;
            }
        }
    }
}
