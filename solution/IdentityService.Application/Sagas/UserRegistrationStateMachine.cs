using MassTransit;
using Shared.Messages;

namespace IdentityService.Application.Sagas
{
    public class UserRegistrationStateMachine : MassTransitStateMachine<UserRegistrationSaga>
    {
        public State Initial { get; private set; }
        public State ProfileCreated { get; private set; }
        public State PostCreated { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }
        
        public Event<RegisterUserStarted> RegisterUserStarted { get; private set; }
        public Event<UserProfileCreated> UserProfileCreated { get; private set; }
        public Event<PostCreated> PostCreated { get; private set; }
        public Event<UserProfileCreationFailed> UserProfileCreationFailed { get; private set; }
        public Event<PostCreationFailed> PostCreationFailed { get; private set; }

        public UserRegistrationStateMachine()
        {
            InstanceState(x => x.CurrentState);
            
            Event(() => RegisterUserStarted, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserProfileCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => PostCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserProfileCreationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => PostCreationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
            
            Initially(
                When(RegisterUserStarted)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.CorrelationId;
                        context.Saga.Email = context.Message.Email;
                        context.Saga.Name = context.Message.Name;
                    })
                    .Publish(context => new CreateUserProfileCommand(context.Saga.CorrelationId, context.Saga.Email, context.Saga.Name))
                    .TransitionTo(ProfileCreated)
            );

            During(ProfileCreated,
                When(UserProfileCreated)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                    })
                    .Publish(context => new CreatePostCommand(context.Saga.CorrelationId, context.Saga.Email))
                    .TransitionTo(PostCreated),
                When(UserProfileCreationFailed)
                    .Then(context =>
                    {
                    })
                    .Publish(context => new RegisterUserFailed(context.Saga.CorrelationId, context.Message.Reason))
                    .TransitionTo(Failed)
            );

            During(PostCreated,
                When(PostCreated)
                    .Then(context =>
                    {
                        context.Saga.PostId = context.Message.PostId;
                    })
                    .Publish(context => new RegisterUserCompleted(context.Saga.CorrelationId))
                    .TransitionTo(Completed),
                When(PostCreationFailed)
                    .Then(context =>
                    {
                    })
                    .Publish(context => new RegisterUserFailed(context.Saga.CorrelationId, context.Message.Reason))
                    .TransitionTo(Failed)
            );
            
            During(Completed,
            );
            
            During(Failed,
            );
        }
    }
}