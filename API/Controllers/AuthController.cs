using Core.DTOs;
using Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
namespace Core.Controllers;

[ApiController]
[Route("api/1/Auth")]
public class AuthController:ControllerBase
{
    IAuthService _authService;
    ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        if(request is null)return BadRequest("Invalid request");

        var res=await _authService.LoginAsync(request);

        if (res is null)return ValidationProblem();

        return Ok(res);
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Register(RegisterRequest registerRequest)
    {
        if(registerRequest is null)return BadRequest("Invalid request");

        var res=await _authService.RegisterAsync(registerRequest);

        if(res is null)return BadRequest("this email is already exist");
        _logger.LogInformation($"{res.UserID}: {res.PersonName}({res.Gender})");
        return Ok(res);
    }

    [HttpPost("RefreshTokensAsync")]
    public async Task<ActionResult> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        if(request is null)return BadRequest("Invalid request");

        var res=await _authService.RefreshTokensAsync(request);

        if (res is null)return ValidationProblem();

        return Ok(res);
    }
}
