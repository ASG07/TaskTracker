using Microsoft.AspNetCore.Identity;
using webapiemp.DTOs;
using webapiemp.Models;

namespace webapiemp.Services;

public interface IIdentityServices
{
    Task<AuthenticationResult> GenerateTokenAsync(User newUser);
    Task<AuthenticationResult> RegisterAsync(string email, string username, string password);
    Task<AuthenticationResult> LoginAsync(string email, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    Task<User> GetUserInfoAsync(int userId);
    Task<AuthenticationResult> ExternalLoginAsync(ExternalLoginInfo loginInfo);
}
