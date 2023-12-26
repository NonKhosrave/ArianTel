using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using BuildingBlocks.SeriLog;
using K4os.Compression.LZ4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace BuildingBlocks.Service;
public static partial class Extensions
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        },
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static readonly JsonSerializerOptions JsonSerializerOptionsWithoutEnumString = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
#pragma warning disable CA1802
    private static readonly RegexOptions RegexOptions =
        RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase;
#pragma warning restore CA1802


    private static readonly Regex NumberRegex = new(@"[\d-]", RegexOptions, TimeSpan.FromMilliseconds(50));

    private static readonly DateTime Epoch = DateTime.UnixEpoch;


    public static readonly Regex MatchIranianPostalCode =
        new("\\b(?!(\\d)\\1{3})[13-9]{4}[1346-9][1-9](?!0000)[0-9]{4}\\b", RegexOptions, TimeSpan.FromSeconds(1));

    public static HttpContent ToHttpContent(this object requestModel, Encoding encoding = null,
        string mediaType = "application/json")
    {
        return new StringContent(requestModel.ToJson(), encoding ?? Encoding.UTF8, mediaType);
    }

    public static object ToDbValue(this object value)
    {
        return value ?? DBNull.Value;
    }

    public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var attribute = value.GetType().GetField(value.ToString())?
            .GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty(property.ToString())?.GetValue(attribute, null);

        return propValue?.ToString();
    }

    public static HttpMessageHandler ByPassSslCertificate()
    {
        return new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
#pragma warning disable S4830
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
#pragma warning restore S4830
        };
    }

    /// <summary>
    ///     ByPass Ssl Certificate Validation
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHttpClientBuilder AddByPassSslCertificate(this IHttpClientBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder.ConfigurePrimaryHttpMessageHandler(ByPassSslCertificate);

        return builder;
    }


    public static string ToJson(this object source) => JsonSerializer.Serialize(source, JsonSerializerOptions);

    public static string ToJson(this object source, JsonSerializerOptions options) => JsonSerializer.Serialize(source, options);

    public static string ToJsonWithLengthLimit(this object source, int maxlength = 4096, JsonSerializerOptions options = null)
    {
        var serialize = JsonSerializer.Serialize(source, options ?? JsonSerializerOptions);
        var length = Math.Min(maxlength, serialize.Length);
        return serialize[..length];
    }

    public static byte[] ToJsonUtf8Bytes(this object source) => JsonSerializer.SerializeToUtf8Bytes(source, JsonSerializerOptions);

    public static byte[] ToJsonUtf8Bytes(this object source, JsonSerializerOptions options) => JsonSerializer.SerializeToUtf8Bytes(source, options);

    public static T ToObject<T>(this string source) => JsonSerializer.Deserialize<T>(source, JsonSerializerOptions);

    public static T ToObject<T>(this string source, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(source, options);

    public static T GetHeader<T>(this HttpRequest request, string headerName, bool isRequired = true)
    {
        try
        {
            var header = request.Headers.First(r => r.Key.Equals(headerName, StringComparison.OrdinalIgnoreCase))
                .Value;

            return (T)Convert.ChangeType(header.ToString(), typeof(T), CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            if (isRequired)
                throw new CustomException("InvalidHeader", $"not found header key: {headerName}");
        }

        return default;
    }

    public static string ToPersianDate(this DateTime date)
    {
        var pc = new PersianCalendar();
        var y = pc.GetYear(date);
        var m = pc.GetMonth(date).ToString(CultureInfo.InvariantCulture);
        var d = pc.GetDayOfMonth(date).ToString(CultureInfo.InvariantCulture);
        var hour = pc.GetHour(date);
        var min = pc.GetMinute(date);
        var second = pc.GetSecond(date);
        //2019-11-20 09:16:16.8244483
        return $"{y}/{m.PadLeft(2, '0')}/{d.PadLeft(2, '0')} {hour}:{min}:{second}";
    }

    public static string ToPersianJustDate(this DateTime date)
    {
        var pc = new PersianCalendar();
        var y = pc.GetYear(date);
        var m = pc.GetMonth(date).ToString(CultureInfo.InvariantCulture);
        var d = pc.GetDayOfMonth(date).ToString(CultureInfo.InvariantCulture);

        //2019-11-20 09:16:16.8244483
        return $"{y}/{m.PadLeft(2, '0')}/{d.PadLeft(2, '0')}";
    }

    public static string ToPersianJustDate(this DateTime date, string seperator)
    {
        var pc = new PersianCalendar();
        var y = pc.GetYear(date);
        var m = pc.GetMonth(date).ToString(CultureInfo.InvariantCulture);
        var d = pc.GetDayOfMonth(date).ToString(CultureInfo.InvariantCulture);

        //2019-11-20 09:16:16.8244483
        return $"{y}{seperator}{m.PadLeft(2, '0')}{seperator}{d.PadLeft(2, '0')}";
    }

    public static string MaskMobile(this string mobile) => $"{mobile[..4]}***{mobile.Substring(mobile.Length - 4, 4)}";

    public static NationalCodeValidationResult ValidateNationalCode(string nationalCode)
    {
        if (string.IsNullOrEmpty(nationalCode))
            return NationalCodeValidationResult.IsNull;

        if (nationalCode.Length != 10) return NationalCodeValidationResult.NotTenDigit;

        if (!DigitRegex().IsMatch(nationalCode))
            return NationalCodeValidationResult.NotTenDigit;

        var allDigitEqual = new[]
        {
            "0000000000",
            "1111111111",
            "2222222222",
            "3333333333",
            "4444444444",
            "5555555555",
            "6666666666",
            "7777777777",
            "8888888888",
            "9999999999"
        };
        if (allDigitEqual.Contains(nationalCode)) return NationalCodeValidationResult.AllDigitAreSame;


        var chArray = nationalCode.ToCharArray();
        var char0 = (int)'0';
        var num0 = (chArray[0] - char0) * 10;
        var num2 = (chArray[1] - char0) * 9;
        var num3 = (chArray[2] - char0) * 8;
        var num4 = (chArray[3] - char0) * 7;
        var num5 = (chArray[4] - char0) * 6;
        var num6 = (chArray[5] - char0) * 5;
        var num7 = (chArray[6] - char0) * 4;
        var num8 = (chArray[7] - char0) * 3;
        var num9 = (chArray[8] - char0) * 2;
        var digit = chArray[9] - char0;

        var calculatedDigit = (num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9) % 11;

        var succ = (calculatedDigit < 2 && digit == calculatedDigit) ||
                   (calculatedDigit >= 2 && 11 - calculatedDigit == digit);
        return succ ? NationalCodeValidationResult.Succeeded : NationalCodeValidationResult.CheckDigitFailed;
    }

    public static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        var consoleLog = Environment.GetEnvironmentVariable("ConsoleLog");
        Console.WriteLine($"-- ConsoleLog has value {consoleLog}");
        if (!string.IsNullOrEmpty(consoleLog) && consoleLog.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddDebug();
            builder.AddConsole(r => { r.LogToStandardErrorThreshold = LogLevel.Trace; });
        }
    }

    public static void ConfigureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config,
        string configName)
    {
        var hostEnvironment = hostingContext.HostingEnvironment;

        config.SetBasePath(hostEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true);

        var version = Assembly.GetEntryAssembly().GetName().Version;
        Console.WriteLine($"-- Service API {version}");

        var configServerUrl = Environment.GetEnvironmentVariable("ConfigServer");

        if (!string.IsNullOrEmpty(configServerUrl))
        {
            Console.WriteLine("-- " + configName + " Config Server uri is " + configServerUrl);

            config.AddConfigServer(new ConfigServerClientSettings
            {
                Name = configName,
                Uri = configServerUrl,
                FailFast = !string.IsNullOrEmpty(configName),
                ValidateCertificates = false,
                Environment = hostEnvironment.EnvironmentName.ToLower(CultureInfo.InvariantCulture)
            });
        }

        config.AddEnvironmentVariables();
    }

    public static void ConfigureKestrel(KestrelServerOptions options)
    {
        options.Limits.Http2.InitialConnectionWindowSize = 2097152;
        options.Limits.Http2.InitialStreamWindowSize = 1048576;
    }

    public static string En2Fa(this string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return string.Empty;

        var length = data.Length;
        return string.Create(length, data, (chars, context) =>
        {
            for (var i = 0; i < length; i++)
                chars[i] = context[i] switch
                {
                    '0' => '\u06F0',
                    '\u0660' => '\u06F0',
                    '1' => '\u06F1',
                    '\u0661' => '\u06F1',
                    '2' => '\u06F2',
                    '\u0662' => '\u06F2',
                    '3' => '\u06F3',
                    '\u0663' => '\u06F3',
                    '4' => '\u06F4',
                    '\u0664' => '\u06F4',
                    '5' => '\u06F5',
                    '\u0665' => '\u06F5',
                    '6' => '\u06F6',
                    '\u0666' => '\u06F6',
                    '7' => '\u06F7',
                    '\u0667' => '\u06F7',
                    '8' => '\u06F8',
                    '\u0668' => '\u06F8',
                    '9' => '\u06F9',
                    '\u0669' => '\u06F9',
                    _ => context[i]
                };
        });
    }

    public static string Fa2En(this string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return string.Empty;

        var length = data.Length;
        return string.Create(length, data, (chars, context) =>
        {
            for (var i = 0; i < length; i++)
                chars[i] = context[i] switch
                {
                    '\u06F0' => '0',
                    '\u0660' => '0',
                    '\u06F1' => '1',
                    '\u0661' => '1',
                    '\u06F2' => '2',
                    '\u0662' => '2',
                    '\u06F3' => '3',
                    '\u0663' => '3',
                    '\u06F4' => '4',
                    '\u0664' => '4',
                    '\u06F5' => '5',
                    '\u0665' => '5',
                    '\u06F6' => '6',
                    '\u0666' => '6',
                    '\u06F7' => '7',
                    '\u0667' => '7',
                    '\u06F8' => '8',
                    '\u0668' => '8',
                    '\u06F9' => '9',
                    '\u0669' => '9',
                    _ => context[i]
                };
        });
    }

    public static long ToUnixTimeStampAsSeconds(this DateTime value)
    {
        var elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalSeconds;
    }

    public static long ToUnixTimeStampAsMilliseconds(this DateTime value)
    {
        var elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalMilliseconds;
    }

    public static long ToUnixTimeStampAsMilliseconds(this DateTime? value)
    {
        if (!value.HasValue)
            return 0;

        var elapsedTime = value.Value - Epoch;
        return (long)elapsedTime.TotalMilliseconds;
    }

    public static DateTime UnixTimestampMillisecondToDateTime(this long unixTimeStamp) => Epoch.AddMilliseconds(unixTimeStamp);

    public static DateTime UnixTimestampSecondToDateTime(this long unixTimeStamp) => Epoch.AddSeconds(unixTimeStamp);

    public static bool IsMelliCard(this string cardNo)
    {
        if (string.IsNullOrEmpty(cardNo)) return false;

        if (cardNo.Trim().Length != 16) return false;

        return cardNo.StartsWith("603799", StringComparison.OrdinalIgnoreCase); // Meli's Card PreNo
    }

    public static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || !IsValidMobilePhoneNumber(phoneNumber)) return phoneNumber;

        phoneNumber = $"0{NormalizeMobilePhoneNumber(phoneNumber)}";

        return $"{phoneNumber[..6]}****{phoneNumber[9..]}";
    }

    public static string ToStringNumber(this Enum value)
    {
        var typedValue = Convert.ChangeType(value, value.GetTypeCode(), CultureInfo.InvariantCulture);
        return typedValue.ToString();
    }

    public static IHttpClientBuilder AddRequestIdHandler(
        this IHttpClientBuilder builder)
    {
        return builder.AddHttpMessageHandler<RequestIdHandler>();
    }

    public static bool IsValidIranianPostalCode(this string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return false;
        postalCode = postalCode.Fa2En();
        return !string.IsNullOrWhiteSpace(postalCode) && MatchIranianPostalCode.IsMatch(postalCode);
    }

    public static bool IsJustNumber(this string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        return NumberRegex.IsMatch(content);
    }

    #region PhoneNumber

    private const string PhoneNumberPattern = @"^(\+?98|098|0|0098)?(9\d{9})$";

    private static readonly Regex PhoneNumberRegex = new(PhoneNumberPattern, RegexOptions | RegexOptions.RightToLeft,
        TimeSpan.FromMilliseconds(100));


    public static bool IsValidMobilePhoneNumber(this string mobilePhonenumber) => !string.IsNullOrEmpty(mobilePhonenumber) && PhoneNumberRegex.IsMatch(mobilePhonenumber);


    public static string NormalizeMobilePhoneNumber(this string mobilePhonenumber)
    {
        if (!IsValidMobilePhoneNumber(mobilePhonenumber))
            throw new ArgumentException($"not valid phone number: {mobilePhonenumber}", paramName: nameof(mobilePhonenumber));

        var match = PhoneNumberRegex.Match(mobilePhonenumber);

        if (match.Success && match.Groups.TryGetValue("2", out var value))
            return value.Value;

        return string.Empty;
    }

    #endregion PhoneNumber

    /// <summary>
    /// Determining the validity of the Shamsi date
    /// </summary>
    /// <param name="persianYear">Year of Shamsi</param>
    /// <param name="persianMonth">Month of Shamsi</param>
    /// <param name="persianDay">ay of Shamsi</param>
    /// <returns></returns>
    public static bool IsValidPersianDate(this int persianYear, int persianMonth, int persianDay)
    {
        if (persianDay is > 31 or <= 0)
        {
            return false;
        }

        if (persianMonth is > 12 or <= 0)
        {
            return false;
        }

        if (persianMonth <= 6 && persianDay > 31)
        {
            return false;
        }

        if (persianMonth >= 7 && persianDay > 30)
        {
            return false;
        }

        if (persianMonth == 12)
        {
            var persianCalendar = new PersianCalendar();
            var isLeapYear = persianCalendar.IsLeapYear(persianYear);

            if (isLeapYear && persianDay > 30)
            {
                return false;
            }

            if (!isLeapYear && persianDay > 29)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Converts a Shamsi Date To Milady Date
    /// </summary>
    /// <param name="persianDateTime">Shamsi date and time</param>
    /// <param name="convertToUtc">Converts the value of the current DateTime to Coordinated Universal Time (UTC)</param>
    /// <param name="beginningOfCentury">Start of Century, if two digits</param>
    /// <returns></returns>
    public static DateTime? ToGregorianDateTime(this string persianDateTime, bool convertToUtc = false, int beginningOfCentury = 1300)
    {
        if (persianDateTime is null)
        {
            return null;
        }

        var splittedDateTime = persianDateTime.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var rawTime = Array.Find(splittedDateTime, s => s.Contains(':', StringComparison.OrdinalIgnoreCase));
        var rawDate = Array.Find(splittedDateTime, s => !s.Contains(':', StringComparison.OrdinalIgnoreCase));

        var splittedDate = rawDate?.Split('/', ',', '؍', '.', '-');
        if (splittedDate?.Length != 3)
        {
            return null;
        }

        var day = GetDay(splittedDate[2]);
        if (!day.HasValue)
        {
            return null;
        }

        var month = GetMonth(splittedDate[1]);
        if (!month.HasValue)
        {
            return null;
        }

        var year = GetYear(splittedDate[0], beginningOfCentury);
        if (!year.HasValue)
        {
            return null;
        }

        if (!IsValidPersianDate(year.Value, month.Value, day.Value))
        {
            return null;
        }

        var hour = 0;
        var minute = 0;
        var second = 0;

        if (!string.IsNullOrWhiteSpace(rawTime))
        {
            var splittedTime = rawTime.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            hour = int.Parse(splittedTime[0], CultureInfo.InvariantCulture);
            minute = int.Parse(splittedTime[1], CultureInfo.InvariantCulture);
            if (splittedTime.Length > 2)
            {
                var lastPart = splittedTime[2].Trim();
                if (!int.TryParse(lastPart, NumberStyles.Number, CultureInfo.InvariantCulture, out second))
                {
                    second = 0;
                }
            }
        }

        var persianCalendar = new PersianCalendar();
        var dateTime = persianCalendar.ToDateTime(year.Value, month.Value, day.Value, hour, minute, second, 0);
        if (convertToUtc)
        {
            dateTime = dateTime.ToUniversalTime();
        }
        return dateTime;
    }

    private static int? GetDay(string part)
    {
        var day = part.ToNumber();
        if (!day.Item1) return null;
        var pDay = day.Item2;
        if (pDay is 0 or > 31) return null;
        return pDay;
    }

    private static int? GetMonth(string part)
    {
        var month = part.ToNumber();
        if (!month.Item1) return null;
        var pMonth = month.Item2;
        if (pMonth is 0 or > 12) return null;
        return pMonth;
    }

    private static int? GetYear(string part, int beginningOfCentury)
    {
        var year = part.ToNumber();
        if (!year.Item1) return null;
        var pYear = year.Item2;
        if (part.Length == 2) pYear += beginningOfCentury;
        return pYear;
    }

    private static Tuple<bool, int> ToNumber(this string data)
    {
        var result = int.TryParse(data, NumberStyles.Number, CultureInfo.InvariantCulture, out var number);
        return new Tuple<bool, int>(result, number);
    }

    [GeneratedRegex(@"\d{10}", RegexOptions.Compiled | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    public static partial Regex DigitRegex();

    public static byte[] Lz4Compress(this byte[] bytes, LZ4Level level = LZ4Level.L00_FAST)
    {
        var source = bytes.AsSpan();
        var target = new byte[LZ4Codec.MaximumOutputSize(source.Length) + 4].AsSpan();
        var size = BitConverter.GetBytes(source.Length).AsSpan();
        size.CopyTo(target);
        var compressedBytesSize = LZ4Codec.Encode(source, target[4..], level);
        return target[..(compressedBytesSize + 4)].ToArray();
    }

    public static byte[] Lz4Compress(this string data, LZ4Level level = LZ4Level.L00_FAST)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        var source = bytes.AsSpan();
        var target = new byte[LZ4Codec.MaximumOutputSize(source.Length) + 4].AsSpan();
        var size = BitConverter.GetBytes(source.Length).AsSpan();
        size.CopyTo(target);
        var compressedBytesSize = LZ4Codec.Encode(source, target[4..], level);
        return target[..(compressedBytesSize + 4)].ToArray();
    }

    public static byte[] Lz4Compress(this object data, LZ4Level level = LZ4Level.L00_FAST)
    {
        var bytes = data.ToJsonUtf8Bytes();
        var source = bytes.AsSpan();
        var target = new byte[LZ4Codec.MaximumOutputSize(source.Length) + 4].AsSpan();
        var size = BitConverter.GetBytes(source.Length).AsSpan();
        size.CopyTo(target);
        var compressedBytesSize = LZ4Codec.Encode(source, target[4..], level);
        return target[..(compressedBytesSize + 4)].ToArray();
    }

    public static byte[] Lz4Decompress(this byte[] compressedBytes)
    {
        var source = compressedBytes.AsSpan();
        var size = source[..4].ToArray();
        var length = BitConverter.ToInt32(size, 0);
        var target = new byte[length].AsSpan();
        var decoded = LZ4Codec.Decode(source[4..], target);
        return target[..decoded].ToArray();
    }
}

public enum DisplayProperty
{
    Description,
    GroupName,
    Name,
    Prompt,
    ShortName,
    Order
}

public enum NationalCodeValidationResult
{
    Succeeded = 0,
    NotTenDigit = 1,
    AllDigitAreSame = 2,
    CheckDigitFailed = 3,
    IsNull = 4
}
