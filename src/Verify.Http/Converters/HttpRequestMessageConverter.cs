using System.Net.Http;
using Newtonsoft.Json;
using VerifyTests;

class HttpRequestMessageConverter :
    WriteOnlyJsonConverter<HttpRequestMessage>
{
    public override void WriteJson(JsonWriter writer, HttpRequestMessage request, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName("Version");
        serializer.Serialize(writer, request.Version);
        writer.WritePropertyName("Method");
        serializer.Serialize(writer, request.Method);
        writer.WritePropertyName("Uri");
        serializer.Serialize(writer, request.RequestUri);

        WriteHeaders(writer, serializer, request);

        WriteCookies(writer, serializer, request);

        writer.WriteEndObject();
    }

    static void WriteCookies(JsonWriter writer, JsonSerializer serializer, HttpRequestMessage request)
    {
        var cookies = request.Headers.Cookies();
        if (!cookies.Any())
        {
            return;
        }

        writer.WritePropertyName("Cookies");
        serializer.Serialize(writer, cookies);
    }

    static void WriteHeaders(JsonWriter writer, JsonSerializer serializer, HttpRequestMessage request)
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