using System.Net.Http;
using Newtonsoft.Json;
using VerifyTests;

class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void WriteJson(JsonWriter writer, HttpResponseMessage response, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName("Version");
        serializer.Serialize(writer, response.Version);
        writer.WritePropertyName("StatusCode");
        writer.WriteValue(response.StatusCode);
        writer.WritePropertyName("IsSuccessStatusCode");
        writer.WriteValue(response.IsSuccessStatusCode);
        writer.WritePropertyName("ReasonPhrase");
        writer.WriteValue(response.ReasonPhrase);

        WriteHeaders(writer, serializer, response);

        WriteCookies(writer, serializer, response);

        writer.WritePropertyName("Request");
        serializer.Serialize(writer, response.RequestMessage);

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