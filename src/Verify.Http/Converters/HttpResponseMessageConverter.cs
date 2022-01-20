using Newtonsoft.Json;

class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void WriteJson(JsonWriter writer, HttpResponseMessage response, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Version");
        serializer.Serialize(writer, response.Version);
        writer.WritePropertyName("Status");
        writer.WriteValue(response.StatusText());

        WriteHeaders(writer, serializer, response);

        WriteCookies(writer, serializer, response);
#if NET5_0_OR_GREATER || NETSTANDARD2_1
        WriteTrailingHeaders(writer, serializer, response);
#endif
        writer.WritePropertyName("Content");
        serializer.Serialize(writer, response.Content);
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

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    static void WriteTrailingHeaders(JsonWriter writer, JsonSerializer serializer, HttpResponseMessage response)
    {
        if (!response.TrailingHeaders.Any())
        {
            return;
        }

        writer.WritePropertyName("TrailingHeaders");
        serializer.Serialize(writer, response.TrailingHeaders);
    }
#endif
}