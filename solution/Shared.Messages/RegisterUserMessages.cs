namespace Shared.Messages
{
    public record RegisterUserStarted(
        Guid CorrelationId,
        string Email,
        string Name
    );
    
    public record CreateUserProfileCommand(
        Guid CorrelationId,
        string Email,
        string Name
    );
    
    public record CreatePostCommand(
        Guid CorrelationId,
        string OwnerEmail
    );
    
    public record UserProfileCreated(
        Guid CorrelationId,
        string UserId
    );
    
    public record PostCreated(
        Guid CorrelationId,
        string PostId
    );
    
    public record UserProfileCreationFailed(
        Guid CorrelationId,
        string Reason
    );
    
    public record PostCreationFailed(
        Guid CorrelationId,
        string Reason
    );
    
    public record RegisterUserCompleted(
        Guid CorrelationId
    );
    
    public record RegisterUserFailed(
        Guid CorrelationId,
        string Reason
    );
}