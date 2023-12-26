using System;
using ArianTel.Core.Config.Ipg;
using Microsoft.AspNetCore.Identity;

namespace ArianTel.Core.Config;
public sealed class AppConfig
{
    public ConnectionStrings ConnectionStrings { get; set; }
    public int NotAllowedPreviouslyUsedPasswords { get; set; }
    public int ChangePasswordReminderDays { get; set; }
    public bool EnableEmailConfirmation { get; set; }
    public string[] EmailsBanList { get; set; }
    public string[] PasswordsBanList { get; set; }
    public CookieOptions CookieOptions { get; set; }
    public LockoutOptions LockoutOptions { get; set; }
    public PasswordOptions PasswordOptions { get; set; }
    public TimeSpan EmailConfirmationTokenProviderLifespan { get; set; }
    public OtpConfig OtpConfig { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public SmsConfig Sms { get; set; }
    public GatewaySettings GatewaySettings { get; set; }
    public SamanProviderSettings SamanProviderSettings { get; set; }
}
