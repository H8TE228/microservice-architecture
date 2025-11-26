namespace Shared.Messages
{
    public record MatchStarted(
        Guid CorrelationId,
        string UserAId,
        string UserBId 
        );
    
    public record CreateChatCommand(
        Guid CorrelationId,
        string UserAId,
        string UserBId
    );
    
    public record ChatCreated(
        Guid CorrelationId,
        string ChatId
    );
    
    public record ChatCreationFailed(
        Guid CorrelationId,
        string Reason
    );
    
    public record MatchCompleted(
        Guid CorrelationId
    );
    
    public record MatchFailed(
        Guid CorrelationId,
        string Reason
    );
}