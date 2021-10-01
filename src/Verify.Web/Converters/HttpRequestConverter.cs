using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VerifyTests;

class HttpRequestConverter :
    WriteOnlyJsonConverter<HttpRequest>
{
    public override void WriteJson(JsonWriter writer, HttpRequest? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        writer.WriteStartObject();

        WriteProperties(writer, serializer, value);

        writer.WriteEndObject();
    }

    public static void WriteProperties(JsonWriter writer, JsonSerializer serializer, HttpRequest request)
    {
        WriteHeaders(writer, serializer, request);

        WriteCookies(writer, serializer, request);
    }

    static void WriteCookies(JsonWriter writer, JsonSerializer serializer, HttpRequest request)
    {
        var cookies = request.Headers.Cookies();
        if (!cookies.Any())
        {
            return;
        }

        writer.WritePropertyName("Cookies");
        serializer.Serialize(writer, cookies);
    }

    static void WriteHeaders(JsonWriter writer, JsonSerializer serializer, HttpRequest request)
    {
        var headers = request.Headers.NotCookies();
        if (!headers.Any())
        {
            return;
        }

        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, headers);
    }
}