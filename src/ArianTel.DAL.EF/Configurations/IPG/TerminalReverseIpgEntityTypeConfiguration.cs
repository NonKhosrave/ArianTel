using System;
using ArianTel.Core.Entities.Ipg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Ipg;
public sealed class TerminalReverseIpgRequestEntityTypeConfiguration : IEntityTypeConfiguration<TerminalReverseIpgRequest>
{
    public void Configure(EntityTypeBuilder<TerminalReverseIpgRequest> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(r => r.ReferenceNo).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TerminalId).HasColumnType("char(11)");
        builder.Property(r => r.TrackingCode).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.RRN).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.PhoneNumber).HasMaxLength(11).IsUnicode(false);
    }
}

public sealed class TerminalReverseIpgResponseEntityTypeConfiguration : IEntityTypeConfiguration<TerminalReverseIpgResponse>
{
    public void Configure(EntityTypeBuilder<TerminalReverseIpgResponse> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Property(r => r.ReferenceNo).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TerminalId).HasColumnType("char(11)");
        builder.Property(r => r.TrackingCode).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.RRN).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.TraceNo).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.MaskedPan).HasMaxLength(16).IsUnicode(false);
        builder.Property(r => r.ErrorCode).HasMaxLength(128).IsUnicode(false);
        builder.Property(r => r.ExtraData).HasMaxLength(1024);
        builder.Property(r => r.ErrorMessage).HasMaxLength(256);
        builder.Property(r => r.PhoneNumber).HasMaxLength(11).IsUnicode(false);
    }
}
