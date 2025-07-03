namespace SpireCore.API.JWT.UserIdentity;

public class JwtUser : IJwtUser
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}
