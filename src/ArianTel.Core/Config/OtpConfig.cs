namespace ArianTel.Core.Config;
public sealed class OtpConfig
{
    public int ExpireAddSeconds { get; set; }
    public byte DigitNumber { get; set; }
    public string Body { get; set; }
}
