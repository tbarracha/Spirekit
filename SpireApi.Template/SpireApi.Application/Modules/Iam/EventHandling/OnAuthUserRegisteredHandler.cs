using SpireApi.Application.Modules.Iam.Domain.Users.Models;
using SpireApi.Application.Modules.Iam.Domain.Users.Repositories;
using SpireApi.Contracts.Events.Authentication;
using SpireCore.Constants;
using SpireCore.Events.Dispatcher;

namespace SpireApi.Application.Modules.Iam.EventHandling;

public class OnAuthUserRegisteredHandler : IEventHandler<AuthUserRegisteredEvent>
{
    private readonly IamUserRepository _iamUserRepository;

    public OnAuthUserRegisteredHandler(IamUserRepository iamUserRepository)
    {
        _iamUserRepository = iamUserRepository;
    }

    public async Task HandleEventAsync(AuthUserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        var exists = await _iamUserRepository.GetFilteredAsync(
            u => u.AuthUserId == @event.AuthUserId,
            state: StateFlags.ACTIVE
        );

        if (exists is null)
        {
            var iamUser = new IamUser
            {
                Id = Guid.NewGuid(),
                AuthUserId = @event.AuthUserId,
                Email = @event.Email,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                DisplayName = $"{@event.FirstName} {@event.LastName}".Trim(),
                UserName = @event.Email,
                StateFlag = StateFlags.ACTIVE
            };

            var newUser = await _iamUserRepository.AddAsync(iamUser);
        }
    }
}
