using PostService.Application.DTOs; // Наш DTO для IdentityUserResponse
using System;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces.IServices
{
    public interface IIdentityServiceGateway
    {
        Task<IdentityUserResponse?> GetUserProfileAsync(Guid userId);
    }
}