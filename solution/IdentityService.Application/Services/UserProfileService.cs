using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Application.Interfaces.IServices;
using IdentityService.API.DTOs;
using IdentityService.Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserProfileService> _logger;
        private const int MAX_PHOTOS = 5;

        public UserProfileService(IUnitOfWork unitOfWork, ILogger<UserProfileService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Attempt to retrieve profile for non-existent user {UserId}.", userId);
                return null;
            }

            _logger.LogInformation("Retrieved profile for user {UserId}.", userId);

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                ZodiacSign = user.ZodiacSign,
                City = user.City,
                Education = user.Education,
                Interests = user.Interests,
                RegistrationDate = user.RegistrationDate,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive,
                Photos = user.Photos
                             .OrderBy(p => p.Order)
                             .Select(p => new UserPhotoDto
                             {
                                 Id = p.Id,
                                 Url = p.Url,
                                 Order = p.Order,
                                 IsMain = p.IsMain,
                                 UploadDate = p.UploadDate
                             }).ToList()
            };
        }

        public async Task<ICollection<UserPhotoDto>> GetUserPhotosAsync(Guid userId)
        {
            var photos = await _unitOfWork.UserPhotos.GetPhotosByUserIdAsync(userId);
            _logger.LogInformation("Retrieved {Count} photos for user {UserId}.", photos.Count, userId);
            return photos.Select(p => new UserPhotoDto
            {
                Id = p.Id,
                Url = p.Url,
                Order = p.Order,
                IsMain = p.IsMain,
                UploadDate = p.UploadDate
            }).ToList();
        }

        public async Task<UserPhotoDto?> UploadPhotoAsync(Guid userId, IFormFile file)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Photo upload failed for non-existent user {UserId}.", userId);
                throw new ArgumentException("User not found.");
            }

            var currentPhotoCount = await _unitOfWork.UserPhotos.GetUserPhotoCountAsync(userId);
            if (currentPhotoCount >= MAX_PHOTOS)
            {
                _logger.LogWarning("Photo upload failed for user {UserId}: Maximum photo limit reached.", userId);
                throw new InvalidOperationException($"Could not find photo {photoId} for user {userId}.");
            }
            
            foreach (var p in photos)
            {
                if (p.IsMain)
                {
                    p.IsMain = false;
                    await _unitOfWork.UserPhotos.UpdateAsync(p);
                }
            }
            
            photoToSetMain.IsMain = true;
            await _unitOfWork.UserPhotos.UpdateAsync(photoToSetMain);

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Photo {PhotoId} set as main for user {UserId}.", photoId, userId);
        }

        public async Task DeletePhotoAsync(Guid userId, Guid photoId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Delete photo failed for non-existent user {UserId}.", userId);
                throw new ArgumentException("User not found.");
            }

            var photoToDelete = await _unitOfWork.UserPhotos.GetByIdAsync(photoId);
            if (photoToDelete == null || photoToDelete.UserId != userId)
            {
                _logger.LogWarning("Delete photo failed: Photo {PhotoId} not found or does not belong to user {UserId}.", photoId, userId);
                throw new ArgumentException("Photo not found or does not belong to user.");
            }

            var currentPhotoCount = await _unitOfWork.UserPhotos.GetUserPhotoCountAsync(userId);
            if (currentPhotoCount <= 1)
            {
                _logger.LogWarning("Delete photo failed for user {UserId}: Cannot delete the last photo.", userId);
                throw new InvalidOperationException("Cannot delete the last photo.");
            }
            
            if (photoToDelete.IsMain)
            {
                var remainingPhotos = user.Photos.Where(p => p.Id != photoId).OrderBy(p => p.Order).ToList();
                if (remainingPhotos.Any())
                {
                    var newMainPhoto = remainingPhotos.First();
                    newMainPhoto.IsMain = true;
                    await _unitOfWork.UserPhotos.UpdateAsync(newMainPhoto);
                    _logger.LogInformation("New main photo {NewMainPhotoId} assigned for user {UserId}.", newMainPhoto.Id, userId);
                }
            }

            await _unitOfWork.UserPhotos.DeleteAsync(photoToDelete);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Photo {PhotoId} deleted for user {UserId}.", photoId, userId);
        }
    }
}