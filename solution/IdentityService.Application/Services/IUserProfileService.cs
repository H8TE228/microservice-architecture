using IdentityService.API.DTOs;
using Microsoft.AspNetCore.Http; // Для работы с файлами
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces.IServices
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
        Task<ICollection<UserPhotoDto>> GetUserPhotosAsync(Guid userId);
        Task<UserPhotoDto?> UploadPhotoAsync(Guid userId, IFormFile file);
        Task SetMainPhotoAsync(Guid userId, Guid photoId);
        Task DeletePhotoAsync(Guid userId, Guid photoId);
    }
}