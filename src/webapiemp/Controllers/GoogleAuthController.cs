using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webapiemp.Services;

namespace webapiemp.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IIdentityServices _identityServices;
        public GoogleAuthController(IIdentityServices identityServices) {
            _identityServices = identityServices;
        }
        //https://localhost:3002/signin-google
        [HttpGet("signin-google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") }; // Added for Google login
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme); // Added for Google response
            if (!result.Succeeded) return BadRequest();

            var loginInfo = new ExternalLoginInfo(result.Principal, GoogleDefaults.AuthenticationScheme, result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value, result.Principal.Identity.Name);
            var token = await _identityServices.ExternalLoginAsync(loginInfo);

            return Ok(new { token, loginInfo });
        }

        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        //    if (!result.Succeeded) return BadRequest();

        //    var loginInfo = new ExternalLoginInfo(result.Principal, GoogleDefaults.AuthenticationScheme, result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value, result.Principal.Identity.Name);
        //    var token = await _identityServices.ExternalLoginAsync(loginInfo);

        //    // Instead of returning the token directly, redirect to your React app with the token as a query parameter
        //    return Redirect($"http://localhost:3000/google-callback?token={token.Token}");
        //}
    }
}
