namespace SpireCore.API.JWT.Identity.Users;

public class JwtUser : IJwtUserIdentity
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
