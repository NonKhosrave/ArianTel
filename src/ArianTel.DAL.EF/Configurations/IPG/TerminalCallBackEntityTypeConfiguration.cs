using System;
using ArianTel.Core.Entities.Ipg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArianTel.DAL.EF.Configurations.Ipg;
public sealed class TerminalCallBackEntityTypeConfiguration : IEntityTypeConfiguration<TerminalCallBack>
{
    public void Configure(EntityTypeBuilder<TerminalCallBack> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasKey(r => r.TerminalInitIpgRequestId).HasName("PK_TerminalCallBack");
        builder.Property(r => r.TerminalInitIpgRequestId).ValueGeneratedNever();
        builder.Property(r => r.ReferenceNo).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TerminalId).HasColumnType("char(11)");
        builder.Property(r => r.ReserveNo).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.TrackingCode).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.RRN).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.TraceNo).HasMaxLength(32).IsUnicode(false);
        builder.Property(r => r.ErrorCode).HasMaxLength(128).IsUnicode(false);
        builder.Property(r => r.ErrorMessage).HasMaxLength(256);
        builder.Property(r => r.ProviderStatus).HasMaxLength(128);
        builder.Property(r => r.Token).HasMaxLength(64).IsUnicode(false);
        builder.Property(r => r.SecurePan).HasColumnType("char(16)");
        builder.Property(x => x.InsertDateTime).HasDefaultValueSql("GETDATE()");
    }
}
