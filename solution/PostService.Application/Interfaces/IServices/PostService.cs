using PostService.Application.Interfaces.IRepositories;
using PostService.Application.Interfaces.IServices;
using PostService.Domain.Entities;
using PostService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PostService.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityServiceGateway _identityServiceGateway;
        private readonly ILogger<PostService> _logger;

        public PostService(IUnitOfWork unitOfWork, IIdentityServiceGateway identityServiceGateway, ILogger<PostService> logger)
        {
            _unitOfWork = unitOfWork;
            _identityServiceGateway = identityServiceGateway;
            _logger = logger;
        }

        public async Task<PostDto?> CreatePostAsync(Guid userId, CreatePostRequest request)
        {
            var identityUser = await _identityServiceGateway.GetUserProfileAsync(userId);
            if (identityUser == null)
            {
                _logger.LogWarning("Failed to create post: User {UserId} not found in IdentityService.", userId);
                throw new ArgumentException($"User {userId} not found or inactive.");
            }
            
            var existingActivePost = await _unitOfWork.Posts.GetActivePostByUserIdAsync(userId);
            if (existingActivePost != null)
            {
                 _logger.LogWarning("User {UserId} already has an active post. Deactivating old post.", userId);
                 existingActivePost.IsActive = false;
                 existingActivePost.LastUpdatedAt = DateTime.UtcNow;
                 await _unitOfWork.Posts.UpdateAsync(existingActivePost);
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UserFirstName = identityUser.FirstName,
                UserLastName = identityUser.LastName,
                UserAge = identityUser.Age,
                UserMainPhotoUrl = identityUser.MainPhotoUrl
            };

            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Post {PostId} created for user {UserId}.", post.Id, userId);
            return ToDto(post);
        }

        private PostDto ToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Description = post.Description,
                CreatedAt = post.CreatedAt,
                IsActive = post.IsActive,
                UserFirstName = post.UserFirstName,
                UserLastName = post.UserLastName,
                UserAge = post.UserAge,
                UserMainPhotoUrl = post.UserMainPhotoUrl
            };
        }
    }
}