using CoreLib.HttpLogic.Services;
using CoreLib.HttpLogic.Services.Interfaces;
using PostService.Application.DTOs;
using PostService.Application.Interfaces.IServices;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PostService.Infrastructure.Services
{
    public class IdentityServiceGateway : IIdentityServiceGateway
    {
        private readonly IHttpRequestService _httpRequestService;
        private readonly IConfiguration _configuration;
        private readonly string _identityServiceBaseUrl;

        public IdentityServiceGateway(IHttpRequestService httpRequestService, IConfiguration configuration)
        {
            _httpRequestService = httpRequestService;
            _configuration = configuration;
            _identityServiceBaseUrl = _configuration["ServiceUrls:IdentityService"] 
                                      ?? throw new InvalidOperationException("IdentityServiceBaseUrl is not configured.");
        }

        public async Task<IdentityUserResponse?> GetUserProfileAsync(Guid userId)
        {
            var requestData = new HttpRequestData
            {
                Method = HttpMethod.Get,
                Uri = new Uri($"{_identityServiceBaseUrl}/api/users/{userId}"),
                ContentType = ContentType.ApplicationJson
            };

            var httpConnectionData = new HttpConnectionData
            {
                Timeout = TimeSpan.FromSeconds(10),
                CancellationToken = CancellationToken.None
            };
            
            var response = await _httpRequestService.SendRequestAsync<IdentityUserResponse>(requestData, httpConnectionData);

            if (response.IsSuccessStatusCode && response.Body != null)
            {
                return response.Body;
            }
            
            Console.WriteLine($"Error fetching user profile from IdentityService. Status: {response.StatusCode}, Body: {response.Body}");
            return null;
        }
    }
}