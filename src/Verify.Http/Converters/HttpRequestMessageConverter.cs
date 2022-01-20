using Newtonsoft.Json;

class HttpRequestMessageConverter :
    WriteOnlyJsonConverter<HttpRequestMessage>
{
    public override void Write(
        VerifyJsonWriter writer,
        HttpRequestMessage request,
        JsonSerializer serializer)
    {
        if (request.Method == HttpMethod.Get &&
            UriConverter.ShouldUseOriginalString(request.RequestUri!) &&
#if NET5_0_OR_GREATER
            request.IsDefaultVersionPolicy() &&
#endif
            !request.Headers.Any() &&
            request.Content == null)
        {
            writer.WriteValue(request.RequestUri!.OriginalString);
            return;
        }

        writer.WriteStartObject();

        if (request.Method != HttpMethod.Get)
        {
            writer.WriteProperty(request, _ => _.Method);
        }

        writer.WritePropertyName("Uri");
        serializer.Serialize(writer, request.RequestUri);

        if (!request.IsDefaultVersion())
        {
            writer.WriteProperty(request, _ => _.Version);
        }

#if NET5_0_OR_GREATER

        if (!request.IsDefaultVersionPolicy())
        {
            writer.WriteProperty(request, _ => _.VersionPolicy);
        }

#endif

        WriteHeaders(writer, serializer, request);

        WriteCookies(writer, serializer, request);

        if (request.Content != null)
        {
            writer.WriteProperty(request, _ => _.Content);
        }

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