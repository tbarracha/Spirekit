namespace SpireApi.Contracts.Events.Authentication;

public class AuthUserLoggedInEvent : AuthUserEventBase
{
    public DateTime LoggedInAt { get; set; }
}
