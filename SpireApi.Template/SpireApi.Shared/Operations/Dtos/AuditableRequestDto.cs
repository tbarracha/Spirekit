
namespace SpireApi.Shared.Operations.Dtos;

public class AuditableRequestDto<TRequest>
{
    public string? ActorId { get; set; }
    public TRequest Data { get; set; }
}
