using Bank_Management_System.DTO;
using Bank_Management_System.models;
using System.Security.Claims;

namespace Bank_Management_System.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(UserRegisterModel model);
        Task<(string Token, object User)> LoginAsync(UserLoginModel model);

        Task<UserDto> GetAuthUserAsync(string token);

        Task<bool> LogoutAsync();
    }
}
