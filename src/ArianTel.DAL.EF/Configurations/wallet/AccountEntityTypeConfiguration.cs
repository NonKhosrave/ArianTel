using System;
using ArianTel.Core.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Wallet;
public sealed class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(a => a.Balance).IsRequired();

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("IX_Account_UserId")
            .IsUnique();
    }
}
