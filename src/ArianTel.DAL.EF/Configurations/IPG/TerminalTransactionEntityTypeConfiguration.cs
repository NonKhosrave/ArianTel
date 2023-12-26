using System;
using ArianTel.Core.Entities.Ipg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Ipg;
public sealed class TerminalTransactionEntityTypeConfiguration : IEntityTypeConfiguration<TerminalTransaction>
{
    public void Configure(EntityTypeBuilder<TerminalTransaction> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(r => r.InvoiceId).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.ReferenceNo).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TerminalId).HasColumnType("char(11)");
        builder.Property(r => r.TrackingCode).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.RRN).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.TraceNo).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.MaskedPan).HasMaxLength(16).IsUnicode(false);
        builder.Property(r => r.ErrorCode).HasMaxLength(128).IsUnicode(false);
        builder.Property(r => r.ErrorMessage).HasMaxLength(256);
        builder.Property(r => r.ProviderMessage).HasMaxLength(512);
        builder.Property(r => r.PhoneNumber).HasMaxLength(11).IsUnicode(false);
        builder.Property(r => r.CustomerIp).HasMaxLength(255);
        builder.Property(r => r.UpdateBy).HasMaxLength(64);
        builder.Property(x => x.InsertDateTime).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(c => c.TerminalInitIpgRequestId)
            .HasDatabaseName("IX_TerminalTransaction_TerminalInitIpgRequestId")
            .IsUnique(false)
            .IsClustered(false);

        builder.HasIndex(c => new { c.RRN, c.InsertDateTime })
            .HasDatabaseName("IX_RRN_InsertTime")
            .IsUnique(false)
            .IsClustered(false);

        builder.HasIndex(c => new { c.PhoneNumber, c.InsertDateTime })
            .HasDatabaseName("IX_PhoneNumber_InsertTime")
            .IsUnique(false)
            .IsClustered(false);


    }
}
