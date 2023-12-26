using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace BuildingBlocks.Common;
public static class UtilityHelper
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

    public static string ObjectToJson(this object source)
    {
        return JsonSerializer.Serialize(source, JsonSerializerOptions);
    }

    public static T JsonToObject<T>(this string source)
    {
        return JsonSerializer.Deserialize<T>(source, JsonSerializerOptions);
    }
}
