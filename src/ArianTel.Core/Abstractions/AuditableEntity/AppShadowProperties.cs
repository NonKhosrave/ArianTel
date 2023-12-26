using System;

namespace ArianTel.Core.Abstractions.AuditableEntity;
public sealed class AppShadowProperties
{
    public string UserAgent { set; get; }
    public string UserIp { set; get; }
    public DateTime Now { set; get; }
    public int? UserId { set; get; }
}