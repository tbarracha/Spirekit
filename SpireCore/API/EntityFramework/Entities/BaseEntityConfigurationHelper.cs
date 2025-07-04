﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities;

public static class BaseEntityConfigurationHelper
{
    public static void ConfigureBaseEntity<T>(EntityTypeBuilder<T> builder) where T : class, ICreatedAt, IUpdatedAt, IStateFlag
    {
        builder.Property(nameof(ICreatedAt.CreatedAt)).IsRequired();
        builder.Property(nameof(IUpdatedAt.UpdatedAt)).IsRequired();
        builder.Property(nameof(IStateFlag.StateFlag)).IsRequired().HasMaxLength(1);
    }
}
