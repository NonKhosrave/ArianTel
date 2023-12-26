using System;
using ArianTel.Core.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Wallet;
public sealed class TransferEntityTypeConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(t => t.Amount).IsRequired();
        builder.Property(t => t.Balance).IsRequired();
        builder.Property(t => t.TrackingNo).IsRequired();
        builder.Property(t => t.CreateDateTime).IsRequired();
        builder.Property(t => t.AccountId).IsRequired();

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transfers)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(a => a.AccountId)
            .HasDatabaseName("IX_Transfer_AccountId");
    }
}
