using MassTransit;

namespace IdentityService.Application.Sagas
{
    public class UserRegistrationSaga : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }
    }
}