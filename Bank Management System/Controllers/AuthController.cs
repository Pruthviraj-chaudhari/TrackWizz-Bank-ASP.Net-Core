using Bank_Management_System.DTO;
using Bank_Management_System.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var (token, user) = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized(new { message = user });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, 
                Secure = true, 
                SameSite = SameSiteMode.Strict, 
                Expires = DateTime.UtcNow.AddHours(2) 
            };

            Response.Cookies.Append("Token", token, cookieOptions);

            return Ok(new { 
                message = "User logged in successfully",
                user
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("Token", out var token) || string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Authentication token is missing or invalid." });

     
                var user = await _authService.GetAuthUserAsync(token);

                if (user == null)
                    return Unauthorized(new { message = "Invalid or expired token." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user details.", error = ex.Message });
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync();

            if (!result)
                return StatusCode(500, new { message = "An error occurred during logout." });

            Response.Cookies.Delete("Token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "User logged out successfully" });
        }

    }
}
