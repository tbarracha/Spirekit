using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireCore.Abstractions.Interfaces;
using SpireCore.API.EntityFramework.Entities;
using SpireCore.API.EntityFramework.Entities.Base;

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
