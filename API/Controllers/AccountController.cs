using API.Extensions;
using Core.Contracts.InfrastructureContracts;
using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TestApi.DTOs.ValidationDto;
namespace Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAuthService authService):ControllerBase
{
    [HttpPost("Register")] // POST api/account/Register
    public async Task<IActionResult> Register(RegisterDto request)
    {
        RegisterResult result = await authService.RegisterAsync(request);

        if (!result.Succeeded)
            return ValidationProblem(result.Errors!.ToModelState());

        return Created(); 
    }



    [HttpPost("Login")]// POST api/account/Login
    public async Task<IActionResult> Login(LoginDto request)
    {
        LoginResult result = await authService.LoginAsync(request);
        if (!result.Succeeded)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(result.Error!.Field, result.Error.Description);
            return ValidationProblem(modelState);
        }

        return Ok(result);
    }
}
