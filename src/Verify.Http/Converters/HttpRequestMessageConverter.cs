class HttpRequestMessageConverter :
    WriteOnlyJsonConverter<HttpRequestMessage>
{
    public override void Write(VerifyJsonWriter writer, HttpRequestMessage request)
    {
        if (request.RequestUri != null &&
            request.Method == HttpMethod.Get &&
            UriConverter.ShouldUseOriginalString(request.RequestUri!) &&
            request.IsDefaultVersionPolicy() &&
            !request.Headers.Any() &&
            request.Content == null)
        {
            writer.WriteValue(request.RequestUri!.OriginalString);
            return;
        }

        writer.WriteStartObject();

        if (request.Method != HttpMethod.Get)
        {
            writer.WriteMember(request, request.Method, "Method");
        }

        writer.WriteMember(request, request.RequestUri, "Uri");

        if (!request.IsDefaultVersion())
        {
            writer.WriteMember(request, request.Version, "Version");
        }

        if (!request.IsDefaultVersionPolicy())
        {
            writer.WriteMember(request, request.VersionPolicy, "VersionPolicy");
        }

        WriteHeaders(writer, request);

        WriteCookies(writer, request);

        if (request.Content?.Headers.ContentLength is not 0)
        {
            writer.WriteMember(request, request.Content, "Content");
        }

        writer.WriteEndObject();
    }

    static void WriteCookies(VerifyJsonWriter writer, HttpRequestMessage request)
    {
        var cookies = request.Headers.Cookies();
        if (cookies.Count != 0)
        {
            writer.WriteMember(request, cookies, "Cookies");
        }
    }

    static void WriteHeaders(VerifyJsonWriter writer, HttpRequestMessage request)
    {
        var headers = request.Headers.NotCookies();
        if (headers.Count != 0)
        {
            writer.WriteMember(request, headers, "Headers");
        }
    }
}