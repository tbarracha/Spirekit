namespace SpireApi.Contracts.Events.Authentication;

public class AuthUserRegisteredEvent : AuthUserEventBase
{
    public DateTime RegisteredAt { get; set; }
}
