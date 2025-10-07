using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.IServices;
using IdentityService.API.DTOs;
using IdentityService.Infrastructure.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork, TokenService tokenService, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("Registration failed: User with email {Email} already exists.", request.Email);
                return null;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                DateOfBirth = null,
                Gender = string.Empty,
                ZodiacSign = string.Empty,
                City = string.Empty,
                Education = string.Empty,
                Interests = string.Empty,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("User {UserId} registered successfully.", user.Id);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = accessToken,
                ExpiresIn = 3600,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for email {Email}: Invalid credentials.", request.Email);
                return null;
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("User {UserId} logged in successfully.", user.Id);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = accessToken,
                ExpiresIn = 3600,
                RefreshToken = refreshToken
            };
        }
        

        public async Task LogoutAsync(LogoutRequest request)
        {
            await _tokenService.InvalidateRefreshTokenAsync(request.RefreshToken);
            _logger.LogInformation("User logged out (refresh token invalidated).");
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Change password failed for user {UserId}: User not found.", userId);
                throw new ArgumentException("User not found.");
            }

            if (!PasswordHasher.VerifyPassword(request.OldPassword, user.PasswordHash))
            {
                _logger.LogWarning("Change password failed for user {UserId}: Incorrect old password.", userId);
                throw new ArgumentException("Incorrect old password.");
            }

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Password changed successfully for user {UserId}.", userId);
        }

        public async Task DeleteAccountAsync(Guid userId, DeleteAccountRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Account deletion failed for user {UserId}: User not found.", userId);
                throw new ArgumentException("User not found.");
            }

            if (!PasswordHasher.VerifyPassword(request.PasswordConfirmation, user.PasswordHash))
            {
                _logger.LogWarning("Account deletion failed for user {UserId}: Incorrect password confirmation.", userId);
                throw new ArgumentException("Incorrect password confirmation.");
            }

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();
            
            _logger.LogInformation("Account {UserId} deleted successfully.", userId);
        }
    }
}