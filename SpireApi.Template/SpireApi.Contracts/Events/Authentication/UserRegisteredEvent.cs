namespace SpireApi.Contracts.Events.Authentication;

public class UserRegisteredEvent : UserAuthEventBase
{
    public DateTime RegisteredAt { get; set; }
}
