using System.Net.Http;
using Newtonsoft.Json;
using VerifyTests;

class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void WriteJson(JsonWriter writer, HttpResponseMessage value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        WriteHeaders(writer, serializer, value);

        WriteCookies(writer, serializer, value);

        writer.WriteEndObject();
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