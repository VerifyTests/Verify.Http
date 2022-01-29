using VerifyTests.Http;

class HttpRequestConverter :
    WriteOnlyJsonConverter<HttpRequest>
{
    public override void Write(VerifyJsonWriter writer, HttpRequest request)
    {
        if (request.Method == HttpMethod.Get &&
            UriConverter.ShouldUseOriginalString(request.Uri) &&
#if NET5_0_OR_GREATER
            request.IsDefaultVersionPolicy() &&
#endif
            request.Headers == null &&
            request.ContentHeaders == null &&
            request.ContentStringParsed == null
           )
        {
            writer.WriteValue(request.Uri.OriginalString);
            return;
        }

        writer.WriteStartObject();

        if (request.Method != HttpMethod.Get)
        {
            writer.WriteProperty(request, request.Method, "Method");
        }

        writer.WriteProperty(request, request.Uri, "Uri");

        if (!request.IsDefaultVersion())
        {
            writer.WriteProperty(request, request.Version, "Version");
        }

#if NET5_0_OR_GREATER

        if (!request.IsDefaultVersionPolicy())
        {
            writer.WriteProperty(request, request.VersionPolicy, "VersionPolicy");
        }

#endif

        writer.WriteProperty(request, request.Headers, "Headers");

        writer.WriteProperty(request, request.ContentHeaders, "ContentHeaders");

        if (request.ContentStringParsed is string stringValue)
        {
            writer.WriteProperty(request, stringValue, "ContentString");
        }
        else
        {
            writer.WriteProperty(request, request.ContentStringParsed, "ContentStringParsed");
        }

        writer.WriteEndObject();
    }
}