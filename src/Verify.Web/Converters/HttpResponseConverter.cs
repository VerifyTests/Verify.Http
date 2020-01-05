using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Verify;

class HttpResponseConverter :
    WriteOnlyJsonConverter<HttpResponse>
{
    public override void WriteJson(JsonWriter writer, HttpResponse? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        writer.WriteStartObject();

        WriteProperties(writer, serializer, value);

        writer.WriteEndObject();
    }

    public static void WriteProperties(JsonWriter writer, JsonSerializer serializer, HttpResponse response)
    {
        WriteHeaders(writer, serializer, response);

        WriteCookies(writer, serializer, response);
    }

    static void WriteCookies(JsonWriter writer, JsonSerializer serializer, HttpResponse response)
    {
        var cookies = response.Headers
            .Where(x => x.Key == HeaderNames.SetCookie)
            .Select(x =>
            {
                var stringSegment = x.Value.Single();
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(x=>x.Name.Value,x=>x.Value.Value);
        if (!cookies.Any())
        {
            return;
        }

        writer.WritePropertyName("Cookies");
        serializer.Serialize(writer, cookies);
    }

    static void WriteHeaders(JsonWriter writer, JsonSerializer serializer, HttpResponse response)
    {
        var headers = response.Headers
            .Where(x => x.Key != HeaderNames.SetCookie)
            .ToDictionary(x => x.Key, x => x.Value.ToString());
        if (!headers.Any())
        {
            return;
        }

        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, headers);
    }
}