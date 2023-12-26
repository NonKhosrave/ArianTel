namespace ArianTel.Core.Config;
public sealed class SmsConfig
{
    public string BaseUrl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public int BodyId { get; set; }
    public ServiceErrors Errors { get; set; }
}
