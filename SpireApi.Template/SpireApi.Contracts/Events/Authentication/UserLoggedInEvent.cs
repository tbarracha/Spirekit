namespace SpireApi.Contracts.Events.Authentication;

public class UserLoggedInEvent : UserAuthEventBase
{
    public DateTime LoggedInAt { get; set; }
}
