using MassTransit;
using Shared.Messages;
using IdentityService.Application.Services;

namespace IdentityService.Application.Consumers
{
    public class CreateUserProfileConsumer : IConsumer<CreateUserProfileCommand>
    {
        private readonly IUserProfileService _userProfileService;

        public CreateUserProfileConsumer(IUserProfileService userProfileService) 
        {
            _userProfileService = userProfileService;
        }
        
        }
    }
}