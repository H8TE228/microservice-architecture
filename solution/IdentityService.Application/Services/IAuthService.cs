using IdentityService.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task LogoutAsync(LogoutRequest request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task DeleteAccountAsync(Guid userId, DeleteAccountRequest request);
    }
}