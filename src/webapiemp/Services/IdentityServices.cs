using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapiemp.DTOs;
using webapiemp.Models;
using webapiemp.Options;

namespace webapiemp.Services;

public class IdentityServices : IIdentityServices
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public IdentityServices(ApplicationDbContext context, IConfiguration config, UserManager<User> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _context = context;
        _config = config;
        _jwtSettings = jwtSettings;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public async Task<AuthenticationResult> GenerateTokenAsync(User newUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                new Claim("id", newUser.Id.ToString())
            }),
            Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            JwtId = token.Id,
            UserID = newUser.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthenticationResult
        {
            Token = tokenHandler.WriteToken(token),
            RefreshToken = refreshToken.Token,
            IsSuccess = true
        };
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var validatedToken = GetPrincipalFromToken(token);
        if (validatedToken == null)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() {"invalid Token"}};
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);//.Subtract(_jwtSettings.TokenLifetime);

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return new AuthenticationResult() { ErrorMessages = new List<string>() { "This token has not expired yet" } };
        }

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

        if (storedRefreshToken == null)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() { "This refresh token does not exist" } };
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() { "This refresh token has expired" } };
        }

        if (storedRefreshToken.Invalidated)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() { "This refresh token has been invalidated" } };
        }

        if (storedRefreshToken.Used)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() { "This refresh token has been used" } };
        }

        if (storedRefreshToken.JwtId != jti)
        {
            return new AuthenticationResult { ErrorMessages = new List<string>() { "This refresh token does not match this JWT" } };
        }

        storedRefreshToken.Used = true;
        _context.RefreshTokens.Update(storedRefreshToken);
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
        return await GenerateTokenAsync(user);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, TokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task<AuthenticationResult> RegisterAsync(string email, string username, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new AuthenticationResult {
                ErrorMessages = new List<string>() {"User with this email already exists"} 
            };
        }

        var newUser = new User
        {
            Email = email,
            UserName = username,
        };

        var createdUser = await _userManager.CreateAsync(newUser,password);

        if (!createdUser.Succeeded)
        {
            return new AuthenticationResult
            {
                ErrorMessages = createdUser.Errors.Select(x => x.Description)
            };
        }

        return await GenerateTokenAsync(newUser);
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null)
        {
            return new AuthenticationResult
            {
                // User does not exist
                ErrorMessages = new List<string>() { "User/Password combination is wrong" }
            };
        }

        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!userHasValidPassword)
        {
            return new AuthenticationResult
            {
                ErrorMessages = new List<string>() { "User/Password combination is wrong" }
            };
        }

        return await GenerateTokenAsync(user);
    }

    public async Task<User> GetUserInfoAsync(int userId)
    {
        //var user =
        //_userManager.GetUserAsync(user);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        

        return user;
    }

    // Added method for handling Google login
    public async Task<AuthenticationResult> ExternalLoginAsync(ExternalLoginInfo loginInfo) // Added external login method
    {
        var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
        if (user == null)
        {
            user = new User
            {
                Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                UserName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                //UserName = "A S"

            };
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user, loginInfo);
        }

        return await GenerateTokenAsync(user);
    }
}
