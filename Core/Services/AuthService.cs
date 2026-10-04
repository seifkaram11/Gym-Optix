using System.Security.Cryptography;
using Core.DTOs;
using Core.Entities;
using Core.ServiceContracts;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Core.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Core.Configuration;
using Microsoft.Extensions.Options;

namespace Core.Services;

public class AuthService : IAuthService
{
    IUsersRepository _usersRepository;
    readonly IPasswordHasher<Users> _passwordHasher;
    IMapper _mapper;
    JWTConfiguration _jWTConfiguration;
    IRolesRepository _rolesRepository;
    IUsersService _usersService;

    public AuthService(IUsersRepository usersRepository, IPasswordHasher<Users> passwordHasher, IMapper mapper, IOptions<JWTConfiguration> jWTConfiguration, IRolesRepository rolesRepository, IUsersService usersService)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _jWTConfiguration = jWTConfiguration.Value;
        _rolesRepository = rolesRepository;
        _usersService = usersService;
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest loginRequest)
    {
        if(loginRequest is null)return null;

        var user =await _usersRepository.GetUserByEmailAsync(loginRequest.Email ?? "");

        if(user is null ||
        _passwordHasher.VerifyHashedPassword(user,user.PasswordHash!,loginRequest.Password!) == PasswordVerificationResult.Failed)
            return null;

        return await GenreateTokenResponseAsync(user);

    }

    public async Task<TokenResponse> GenreateTokenResponseAsync(Users user)
    {
        return new TokenResponse
        {
            RefreshToken= await GenerateAndSaveRefreshTokenAsync(user),
            AccessToken= await createToken(user)
        };
    }

    async Task<Users?> ValidateRefreshToken(string RefreshToken,Guid UserId)
    {
        var user=await _usersRepository.GetUserByIDAsync(UserId);

        if(user is null || user.RefreshToken!=RefreshToken || user.RefreshTokenExpiryTime < DateTimeOffset.UtcNow)return null;

        return user;
    }
    async Task<string> GenerateAndSaveRefreshTokenAsync(Users user)
    {
        string theRefreshToken=GenerateRefreshToken();
        user.RefreshToken=theRefreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(3);
        await _usersRepository.UpdateUserAsync(user);
        return theRefreshToken;
    }
    string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
    public async Task<TokenResponse?> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
        if(request is null ||  string.IsNullOrEmpty(request.RefreshToken))return null;
        Users? user=await ValidateRefreshToken(request.RefreshToken,request.UserId);
        if(user is null)return null;

        return await GenreateTokenResponseAsync(user);
    }

    public async Task<AuthenticationResponse?> RegisterAsync(RegisterRequest register)
    {
        string refreshToken=GenerateRefreshToken();
        Users user=_mapper.Map<Users>(register);
        user.UserID=Guid.NewGuid();
        user.RefreshToken=refreshToken;
        if(int.TryParse(_jWTConfiguration.ExpiryDays,out int days))
        {
            user.RefreshTokenExpiryTime=DateTimeOffset.UtcNow.AddDays(days);
        }
        else
        {
            user.RefreshTokenExpiryTime=DateTimeOffset.UtcNow.AddDays(2);
        }
        var res=_mapper.Map<AuthenticationResponse>(user);
        user.PasswordHash=_passwordHasher.HashPassword(user,register.Password!);
        var x=await _usersRepository.AddUserAsync(user);
        if(x is null)return null;
        res.Success=true;
        await _usersRepository.AddRoleToUserAsync(res.UserID, new Guid("D6BC75C9-AD00-4049-97B3-C2A9BA49E1D0"));
        return res;
    }

    async Task<string> createToken(Users user)
    {
        var roles=await _rolesRepository.GetRolesByUserIdAsync(user.UserID);
        var claims=new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name??""),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Gender, user.Gender??""),
            new Claim(ClaimTypes.Email, user.Email?? "empty@empty.com")
        };
        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTConfiguration.Token));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
                issuer: _jWTConfiguration.Issuer,
                audience: _jWTConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

        var res= new JwtSecurityTokenHandler().WriteToken(token);

        return res;
    }

    public async Task<Users> FindOrCreateUserByGoogleId(string googleId, string email)
    {
        var user=await _usersRepository.GetUserByEmailAsync(email);

        if (user == null)
        {
            var newUser = new Users {
                UserID=Guid.NewGuid(),
                GoogleId = googleId,
                Email =  email
            };
            await _usersRepository.AddUserAsync(user);
            AddRoleToUserRequest request= new(){RoleId=new Guid("418d04f1-3b33-4146-9801-de7e373b5d73"),UserId=newUser!.UserID};
            await _usersService.AddRoleAsync(request);
            return newUser;
        }
        return user;
    }
}
