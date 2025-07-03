namespace {Namespace}.Shared.JWT.MicroServiceIdentity;

public class ServiceIdentity : IJwtServiceAuth
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
