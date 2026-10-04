using Core.ServiceContracts;
using Google.Apis.Auth;
using Core.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers;
[ApiController]
[Route("api/V1/GoogleAuth")]
public class GoogleAuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _tokenService;

    public GoogleAuthController( IConfiguration configuration, IAuthService tokenService)
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest model)
    {
        var validGoogleUser = await ValidateGoogleToken(model.IdToken);

        if (validGoogleUser == null)
            return Unauthorized();

        var user =await _tokenService.FindOrCreateUserByGoogleId(validGoogleUser.JwtId, validGoogleUser.Email);
        var jwt =await _tokenService.GenreateTokenResponseAsync(user);

        return Ok(new
        {
            accessToken = jwt.AccessToken,
            refreshToken = jwt.RefreshToken
            });
    }


    private async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string>() { _configuration["Google:ClientId"]! }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Google Token Validation Failed: " + ex.Message);
            return null;
        }
    }


    [HttpGet("login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync();

        if (!result.Succeeded)
            return Unauthorized();

        var claims = result.Principal.Claims.Select(c => new
        {
            c.Type,
            c.Value
        });

        return Ok(claims);
    }
}
