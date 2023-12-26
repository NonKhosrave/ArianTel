using ArianTel.Core.Abstractions;
using ArianTel.Core.Abstractions.AuditableEntity;

namespace ArianTel.Core.Entities.Ipg;
public sealed class TerminalReverseIpgRequest : DbBaseEntity<long>, IAuditableEntity
{
    public string RRN { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string PhoneNumber { get; set; }
    public string ReferenceNo { get; set; }
    public byte BankCode { get; set; }
    public string TrackingCode { get; set; }
}

public sealed class TerminalReverseIpgResponse : DbBaseEntity<long>
{
    public string RRN { get; set; }
    public long Amount { get; set; }
    public string PhoneNumber { get; set; }
    public byte BankCode { get; set; }
    public string TrackingCode { get; set; }
    public bool Succeed { get; set; }
    public string ReferenceNo { get; set; }
    public string TerminalId { get; set; }
    public string MaskedPan { get; set; }
    public string TraceNo { get; set; }
    public string ExtraData { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
