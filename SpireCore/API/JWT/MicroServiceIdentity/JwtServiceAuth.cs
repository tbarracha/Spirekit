namespace SpireCore.API.JWT.MicroServiceIdentity;

public class JwtServiceAuth : IJwtServiceAuth
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public string ServiceName { get; set; } = default!;
    public string? Description { get; set; }
}
