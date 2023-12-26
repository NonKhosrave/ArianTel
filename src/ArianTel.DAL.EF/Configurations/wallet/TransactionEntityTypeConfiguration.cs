using System;
using ArianTel.Core.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Wallet;
public sealed class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(t => t.DesBalance).IsRequired();
        builder.Property(t => t.Amount).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.CreateDateTime).IsRequired();

        builder.HasOne(t => t.SrcAccount)
            .WithMany(a => a.SrcTransactions)
            .HasForeignKey(t => t.SrcAccountId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasOne(t => t.DesAccount)
           .WithMany(a => a.DesTransactions)
           .HasForeignKey(t => t.DesAccountId)
           .OnDelete(DeleteBehavior.NoAction)
           .IsRequired();

        builder.HasIndex(a => a.SrcAccountId)
            .HasDatabaseName("IX_Transaction_SrcAccountId");

        builder.HasIndex(a => a.DesAccountId)
            .HasDatabaseName("IX_Transaction_DesAccountId");
    }
}
