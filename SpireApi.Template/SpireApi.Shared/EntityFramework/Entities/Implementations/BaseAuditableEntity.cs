using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Shared.EntityFramework.Entities;
using SpireApi.Shared.EntityFramework.Entities.Abstractions;
using SpireApi.Shared.EntityFramework.Entities.Implementations;
using SpireCore.Abstractions.Interfaces;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity<TId>
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public override void ConfigureEntity<T>(EntityTypeBuilder<T> builder)
    {
        base.ConfigureEntity(builder);

        if (typeof(ICreatedBy).IsAssignableFrom(typeof(T)))
        {
            BaseEntityConfigurationHelper.ConfigureCreatedBy((EntityTypeBuilder<ICreatedBy>)(object)builder);
        }
        if (typeof(IUpdatedBy).IsAssignableFrom(typeof(T)))
        {
            BaseEntityConfigurationHelper.ConfigureUpdatedBy((EntityTypeBuilder<IUpdatedBy>)(object)builder);
        }
    }
}
