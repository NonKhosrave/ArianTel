using System;
using ArianTel.Core.Entities.Ipg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Ipg;
public sealed class TerminalInitIpgRequestEntityTypeConfiguration : IEntityTypeConfiguration<TerminalInitIpgRequest>
{
    public void Configure(EntityTypeBuilder<TerminalInitIpgRequest> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(r => r.InvoiceId).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.PhoneNumber).HasMaxLength(11).IsUnicode(false);
        builder.Property(r => r.AdditionalData).HasMaxLength(512);
        builder.Property(r => r.CustomerIp).HasMaxLength(255);
        builder.Property(x => x.InsertDateTime).HasDefaultValueSql("GETDATE()");
        builder.Property(r => r.UserAgent).HasMaxLength(1000).IsUnicode(false);

        builder.HasIndex(c => c.InvoiceId)
            .HasDatabaseName("IX_TerminalInitIpgRequest_InvoiceId_Unique")
            .IsClustered(false);
    }
}

public sealed class TerminalInitIpgResponseEntityTypeConfiguration : IEntityTypeConfiguration<TerminalInitIpgResponse>
{
    public void Configure(EntityTypeBuilder<TerminalInitIpgResponse> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasKey(r => r.TerminalInitIpgRequestId).HasName("PK_TerminalInitIpgResponse");
        builder.Property(r => r.TerminalInitIpgRequestId).ValueGeneratedNever();
        builder.Property(r => r.HttpMethod).HasMaxLength(8).IsUnicode(false);
        builder.Property(r => r.Token).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TerminalId).HasMaxLength(8).IsUnicode(false);
        builder.Property(r => r.InvoiceId).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TrackingCode).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.ExtraData).HasMaxLength(1024);
        builder.Property(r => r.ErrorCode).HasMaxLength(128).IsUnicode(false);
        builder.Property(r => r.ErrorMessage).HasMaxLength(256);
        builder.Property(r => r.ProviderMessage).HasMaxLength(512);
        builder.Property(r => r.UrlRedirectForm).HasMaxLength(256);
        builder.Property(r => r.CallBackUrl).HasMaxLength(256);
        builder.Property(r => r.PhoneNumber).HasMaxLength(11).IsUnicode(false);
        builder.Property(r => r.CustomerIp).HasMaxLength(255);
        builder.Property(x => x.InsertDateTime).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(c => c.TrackingCode)
            .HasDatabaseName("IX_TerminalInitIpgResponse_TrackingCode")
            .IsUnique(false)
            .IsClustered(false);
    }
}
