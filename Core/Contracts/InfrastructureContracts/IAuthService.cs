using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using TestApi.DTOs.ValidationDto;

namespace Core.Contracts.InfrastructureContracts
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(RegisterDto registerDto);
        Task<LoginResult> LoginAsync(LoginDto loginDTO);
    }
}
