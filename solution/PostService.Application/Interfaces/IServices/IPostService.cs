using PostService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces.IServices
{
    public interface IPostService
    {
        Task<PostDto?> CreatePostAsync(Guid userId, CreatePostRequest request);
        Task<PostDto?> GetPostByIdAsync(Guid postId);
        Task<ICollection<PostDto>> GetUserPostsAsync(Guid userId);
        Task UpdatePostAsync(Guid userId, Guid postId, UpdatePostRequest request);
        Task DeletePostAsync(Guid userId, Guid postId);
    }
}