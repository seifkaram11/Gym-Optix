using Core.DTOs;
using Core.Entities;

namespace Core.ServiceContracts;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest loginRequest);
    Task<AuthenticationResponse?> RegisterAsync(RegisterRequest users);
    Task<TokenResponse?> RefreshTokensAsync(RefreshTokenRequestDto request);
    Task<Users> FindOrCreateUserByGoogleId(string googleId, string email);
    Task<TokenResponse> GenreateTokenResponseAsync(Users user);
}

