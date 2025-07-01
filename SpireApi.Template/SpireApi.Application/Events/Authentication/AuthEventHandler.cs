using SpireCore.Events.Dispatcher;

namespace SpireApi.Application.Events.Authentication;

// Handles both registration and login events
public class AuthEventHandler :
    IEventHandler<AppUserRegisteredEvent>,
    IEventHandler<AppUserLoggedInEvent>
{
    public async Task HandleEventAsync(AppUserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        // Example: Send welcome email, audit log, etc.
        Console.WriteLine($"[AuthEventHandler] User registered: {@event.UserId}, {@event.Email}, {@event.RegisteredAt:O}");
        await Task.CompletedTask;
    }

    public async Task HandleEventAsync(AppUserLoggedInEvent @event, CancellationToken cancellationToken = default)
    {
        // Example: Log login event, update user session, etc.
        Console.WriteLine($"[AuthEventHandler] User logged in: {@event.UserId}, {@event.Email}, {@event.LoggedInAt:O}, IP: {@event.IpAddress}, UA: {@event.UserAgent}");
        await Task.CompletedTask;
    }
}