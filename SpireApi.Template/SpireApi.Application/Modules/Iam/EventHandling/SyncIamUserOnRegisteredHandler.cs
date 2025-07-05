using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Application.Modules.Iam.Domain.Models.Users.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Services;
using SpireApi.Contracts.Events.Authentication;
using SpireCore.Constants;
using SpireCore.Events.Dispatcher;

namespace SpireApi.Application.Modules.Iam.EventHandling;

public class SyncIamUserOnRegisteredHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly IamUserRepository _iamUserRepository;
    private readonly IamService _iamService;

    public SyncIamUserOnRegisteredHandler(IamUserRepository iamUserRepository, IamService iamService)
    {
        _iamUserRepository = iamUserRepository;
        _iamService = iamService;
    }

    public async Task HandleEventAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
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
            await _iamService.NewUserSetup(newUser.Id);
        }
    }
}
