using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly JWTConfig _jwt;
        public AuthenticationService(UserManager<ApplicationUser> userManager = null, ILogger<AuthenticationService> logger = null, IOptions<JWTConfig> jwtConfig = null)
        {
            _userManager = userManager;
            _logger = logger;
            _jwt = jwtConfig.Value;
        }

        public async Task<AuthenticationModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                _logger.LogInformation("Email is already registered: {Email}", model.Email);
                return new AuthenticationModel { Message = "Email is already registered!" };
            }

            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                _logger.LogInformation("Username is already registered: {Username}", model.Username);
                return new AuthenticationModel { Message = "Username is already registered!" };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                _logger.LogError("Failed to register user. Errors: {Errors}", errors);

                return new AuthenticationModel { Message = errors };
            }

            _ = await _userManager.AddToRoleAsync(user, "user");

            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            _ = await _userManager.UpdateAsync(user);

            _logger.LogInformation("User registered successfully: {Username}", user.UserName);
            // Calculate expiration in seconds for the access token
            var expiresIn = (int)(jwtSecurityToken.ValidTo - DateTime.UtcNow).TotalSeconds;

            // Calculate expiration in seconds for the refresh token
            var refreshTokenExpiresIn = (int)(refreshToken.ExpiresOn - DateTime.UtcNow).TotalSeconds;

            return new AuthenticationModel
            {
                UserId = user.Id,
                Email = user.Email,
                ExpiresOn = expiresIn,
                IsAuthenticated = true,
                Roles = new List<string> { "user" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshTokenExpiresIn
            };

        }
        public async Task<AuthenticationModel> GetTokenAsync(LoginModel model)
        {
            try
            {


                _logger.LogInformation("GetTokenAsync method called.");

                var AuthenticationModel = new AuthenticationModel();

                var user = await _userManager.FindByNameAsync(model.Username);

                if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogInformation("Username or Password is incorrect for user: {Username}", model.Username);
                    AuthenticationModel.Message = "Username or Password is incorrect!";
                    return AuthenticationModel;
                }
                _logger.LogInformation("User authenticated successfully: {Username}", user.UserName);
                var jwtSecurityToken = await CreateJwtToken(user);
                var rolesList = await _userManager.GetRolesAsync(user);

                AuthenticationModel.IsAuthenticated = true;
                AuthenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                AuthenticationModel.Email = user.Email;
                AuthenticationModel.Username = user.UserName;
                // Calculate expiration in seconds
                var expiresIn = (jwtSecurityToken.ValidTo - DateTime.UtcNow).TotalSeconds;

                // Assign expiration in seconds to the AuthenticationModel
                AuthenticationModel.ExpiresOn = (int)expiresIn;
                AuthenticationModel.Roles = rolesList.ToList();

                if (user.RefreshTokens.Any(t => t.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    AuthenticationModel.RefreshToken = activeRefreshToken.Token;
                    // Calculate expiration in seconds for refresh token
                    var refreshTokenExpiresIn = (activeRefreshToken.ExpiresOn - DateTime.UtcNow).TotalSeconds;
                    AuthenticationModel.RefreshTokenExpiration = (int)refreshTokenExpiresIn;
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    AuthenticationModel.RefreshToken = refreshToken.Token;
                    // Calculate expiration in seconds for refresh token
                    var refreshTokenExpiresIn = (refreshToken.ExpiresOn - DateTime.UtcNow).TotalSeconds;
                    AuthenticationModel.RefreshTokenExpiration = (int)refreshTokenExpiresIn;
                    user.RefreshTokens.Add(refreshToken);
                    _ = await _userManager.UpdateAsync(user);
                }


                return AuthenticationModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Response> VerifyUser(LoginModel model)
        {
            _logger.LogInformation("VerifyUser method called.");

            var result = new Response();

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogInformation("Username or Password is incorrect for user: {Username}", model.Username);
                result.Message = "Username or Password is incorrect!";
                return result;
            }

            return result;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation("CreateJwtToken method called for user: {UserName}", user.UserName);

                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        }
                .Union(userClaims)
                .Union(roleClaims);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(_jwt.DurationInMinutes),
                    signingCredentials: signingCredentials);

                _logger.LogInformation("JWT token created successfully for user: {UserName}", user.UserName);

                return jwtSecurityToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CreateJwtToken method for user: {UserName}", user.UserName);
                return null;
            }
        }

        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            try
            {
                _logger.LogInformation("RefreshTokenAsync method called for token: {Token}", token);

                var authModel = new AuthenticationModel();

                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

                if (user == null)
                {
                    authModel.Message = "Invalid token";
                    _logger.LogInformation("Invalid token: {Token}", token);
                    return authModel;
                }

                var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

                if (!refreshToken.IsActive)
                {
                    authModel.Message = "Inactive token";
                    _logger.LogInformation("Inactive token: {Token}", token);
                    return authModel;
                }

                refreshToken.RevokedOn = DateTime.UtcNow;

                var newRefreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                _ = await _userManager.UpdateAsync(user);

                var jwtToken = await CreateJwtToken(user);
                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                authModel.Email = user.Email;
                authModel.Username = user.UserName;
                var roles = await _userManager.GetRolesAsync(user);
                authModel.Roles = roles.ToList();
                authModel.RefreshToken = newRefreshToken.Token;
                var expiresIn = (int)(jwtToken.ValidTo - DateTime.UtcNow).TotalSeconds;
                authModel.ExpiresOn = expiresIn;
                // Calculate expiration in seconds for the new refresh token
                var refreshTokenExpiresIn = (int)(newRefreshToken.ExpiresOn - DateTime.UtcNow).TotalSeconds;
                authModel.RefreshTokenExpiration = refreshTokenExpiresIn;

                _logger.LogInformation("Token refreshed successfully for user: {UserName}", user.UserName);

                return authModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RefreshTokenAsync method for token: {Token}", token);
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                _logger.LogInformation("RevokeTokenAsync method called for token: {Token}", token);

                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

                if (user == null)
                {
                    _logger.LogInformation("User not found for token: {Token}", token);
                    return false;
                }

                var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

                if (!refreshToken.IsActive)
                {
                    _logger.LogInformation("Token already inactive for token: {Token}", token);
                    return false;
                }

                refreshToken.RevokedOn = DateTime.UtcNow;

                _ = await _userManager.UpdateAsync(user);

                _logger.LogInformation("Token revoked successfully for user: {UserName}", user.UserName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RevokeTokenAsync method for token: {Token}", token);
                return false;
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            try
            {
                _logger.LogInformation("Generating refresh token...");

                var randomNumber = new byte[32];

                using var generator = new RNGCryptoServiceProvider();

                generator.GetBytes(randomNumber);

                var refreshToken = new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    ExpiresOn = DateTime.UtcNow.AddDays(10),
                    CreatedOn = DateTime.UtcNow
                };

                _logger.LogInformation("Refresh token generated successfully.");

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating refresh token.");
                // If an error occurs, return a default RefreshToken or null, depending on your needs.
                return null;
            }
        }

    }
}
