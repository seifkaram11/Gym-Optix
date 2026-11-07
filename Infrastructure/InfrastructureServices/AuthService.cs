using Core.Configuration;
using Core.Contracts.InfrastructureContracts;
using Core.DTOs;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApi.DTOs.ValidationDto;

namespace Infrastructure.InfrastructureServices
{
    public class AuthService(
       UserManager<ApplicationUser> userManager,
       IOptions<JwtOptions> jwtOptions,
       SignInManager<ApplicationUser> signInManager) : IAuthService
    {
        private readonly JwtOptions JWTOptions = jwtOptions.Value;
        public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
        {
            // it handles duplicates, if user exists return errors in identityresult  
            var identityResult = await userManager.CreateAsync
            (
                new ApplicationUser()
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                },
                registerDto.Password
            );

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => new AuthError(
                Field: e.Code switch
                {
                    // Email
                    nameof(IdentityErrorDescriber.DuplicateEmail) => "Email",
                    nameof(IdentityErrorDescriber.InvalidEmail) => "Email",

                    // Username
                    nameof(IdentityErrorDescriber.DuplicateUserName) => "Username",
                    nameof(IdentityErrorDescriber.InvalidUserName) => "Username",

                    // Password
                    nameof(IdentityErrorDescriber.PasswordTooShort) => "Password",
                    nameof(IdentityErrorDescriber.PasswordRequiresDigit) => "Password",
                    nameof(IdentityErrorDescriber.PasswordRequiresLower) => "Password",
                    nameof(IdentityErrorDescriber.PasswordRequiresUpper) => "Password",
                    nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => "Password",
                    nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars) => "Password",
                    nameof(IdentityErrorDescriber.PasswordMismatch) => "Password",

                    // Roles
                    nameof(IdentityErrorDescriber.DuplicateRoleName) => "RoleName",
                    nameof(IdentityErrorDescriber.InvalidRoleName) => "RoleName",

                    // Top-level banner fallback ("")
                    _ => string.Empty
                },
                Description: e.Description
             )).ToList();

                return RegisterResult.Failure(errors);
            }
            return RegisterResult.Success();
        }
        public async Task<LoginResult> LoginAsync(LoginDto loginDTO)
        {
            //check username , password
            var DbUser = await userManager.FindByNameAsync(loginDTO.UserName);

            // 1) wrong username
            if (DbUser == null)
                return LoginResult.Failure("InvalidCredentials", "Invalid email or password.");

            // use signInManager to apply check PASSWORD and LOCKOUT on failure
            var result = await signInManager.CheckPasswordSignInAsync(
                DbUser,
                loginDTO.Password,
                lockoutOnFailure: true
            );

            if (result.Succeeded == false)
            {
                if (result.IsLockedOut)
                    return LoginResult.Failure("UserName", "Account is locked due to multiple failed attempts.");

                if (result.IsNotAllowed)
                    return LoginResult.Failure("Email", "You must confirm your email before logging in.");
                
                // 2) wrong password
                return LoginResult.Failure("InvalidCredentials", "Invalid email or password.");
            }

            (string token, DateTime expirationUtc) = await CreateTokenAsync(DbUser);

            return new LoginResult
            {
                Succeeded = true,
                Token = token,
                ExpirationUtc = expirationUtc
            };

        }
        private async Task<(string Token, DateTime ExpirationUtc)> CreateTokenAsync(ApplicationUser DbUser)
        {
            var userRoles = await userManager.GetRolesAsync(DbUser);
            List<Claim> userClaims =
            [
                new Claim(ClaimTypes.NameIdentifier, DbUser.Id),
                new Claim(ClaimTypes.Name, DbUser.UserName),
                ..userRoles.Select(r => new Claim(ClaimTypes.Role, r)),
                 // JWT has its own STANDARD CLAIMS made by IANA , Jti : JWT Id
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTOptions.SecurityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(JWTOptions.ExpiryMinutes);
            JwtSecurityToken myToken = new JwtSecurityToken(
                issuer: JWTOptions.Issuer,
                audience: JWTOptions.Audience,
                expires: expiration,
                claims: userClaims,
                signingCredentials: signingCredentials
                );
            string token = new JwtSecurityTokenHandler().WriteToken(myToken);
            return (token, expiration);
        }


    }
}
