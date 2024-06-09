using Microsoft.AspNetCore.Mvc;
using webapiemp.Models;
using webapiemp.Services;
using Microsoft.AspNetCore.Http;
using webapiemp.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using webapiemp.DTOs.RequestDTO;
using webapiemp.DTOs.ResponseDTO;
using webapiemp.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace webapiemp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : Controller
{

    private readonly IIdentityServices _identityServices;
    private readonly ApplicationDbContext _context;
    public IdentityController(IIdentityServices identityServices, ApplicationDbContext context)
    {
        _identityServices = identityServices;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegister)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse { Errors = ModelState.Values.SelectMany(e => e.Errors.Select(em => em.ErrorMessage)) });

        }

        var authResponse = await _identityServices.RegisterAsync(userRegister.Email, userRegister.Username, userRegister.Password);
        if (!authResponse.IsSuccess)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.ErrorMessages
            });
        }

        return Ok(new AuthSuccessResponseDTO
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken
        });
    }

    //pwd: Aa12345!
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin)
    {
        var authResponse = await _identityServices.LoginAsync(userLogin.Email, userLogin.Password);

        if (!authResponse.IsSuccess)
        {
            return NotFound(new AuthFailedResponse
            {
                Errors = authResponse.ErrorMessages
            });

        }

        return Ok(new AuthSuccessResponseDTO
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO request)
    {
        var authResponse = await _identityServices.RefreshTokenAsync(request.Token, request.RefreshToken);

        if (!authResponse.IsSuccess)
        {
            return NotFound(new AuthFailedResponse
            {
                Errors = authResponse.ErrorMessages
            });

        }

        return Ok(new AuthSuccessResponseDTO
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken
        });
    }

    [HttpGet()]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = HttpContext.GetUserId();
        if (userId == -1) return BadRequest();

        var user = await _identityServices.GetUserInfoAsync(userId);
        if (user == null) return BadRequest();

        var roles = from u in _context.GroupMemberships
                    where u.UserId == userId
                    select new
                    {
                        groupId = u.GroupId,
                        role = u.Role,
                    };




        return Ok(new { userId = user.Id, userName = user.UserName, email = user.Email, roles = roles });
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") }; // Added for Google login
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }



    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded) return BadRequest();

        var loginInfo = new ExternalLoginInfo(result.Principal, GoogleDefaults.AuthenticationScheme, result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value, result.Principal.Identity.Name);
        var token = await _identityServices.ExternalLoginAsync(loginInfo);

        // Instead of returning the token directly, redirect to your React app with the token as a query parameter
        return Redirect($"http://localhost:3000/google-callback?token={token.Token}");
    }


    //[HttpGet("google-response")]
    //public async Task<IActionResult> GoogleResponse()
    //{
    //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme); // Added for Google response
    //    if (!result.Succeeded) return BadRequest();

    //    var loginInfo = new ExternalLoginInfo(result.Principal, GoogleDefaults.AuthenticationScheme, result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value, result.Principal.Identity.Name);
    //    var token = await _identityServices.ExternalLoginAsync(loginInfo);

    //    return Ok(new { token, name = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName) });
    //}
}

