using System.Net.Http;
using Newtonsoft.Json;
using VerifyTests;

class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void WriteJson(JsonWriter writer, HttpResponseMessage? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        writer.WriteStartObject();

        WriteProperties(writer, serializer, value);

        writer.WriteEndObject();
    }

    public static void WriteProperties(JsonWriter writer, JsonSerializer serializer, HttpResponseMessage response)
    {
        WriteHeaders(writer, serializer, response);

        WriteCookies(writer, serializer, response);
    }

    static void WriteCookies(JsonWriter writer, JsonSerializer serializer, HttpResponseMessage response)
    {
        var cookies = response.Headers.Cookies();
        if (!cookies.Any())
        {
            return;
        }

        writer.WritePropertyName("Cookies");
        serializer.Serialize(writer, cookies);
    }

    static void WriteHeaders(JsonWriter writer, JsonSerializer serializer, HttpResponseMessage response)
    {
        var headers = response.Headers.NotCookies();
        if (!headers.Any())
        {
            return;
        }

        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, headers);
    }
}