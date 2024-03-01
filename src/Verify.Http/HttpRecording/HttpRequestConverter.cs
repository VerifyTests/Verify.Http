class HttpRequestConverter :
    WriteOnlyJsonConverter<HttpRequest>
{
    public override void Write(VerifyJsonWriter writer, HttpRequest request)
    {
        if (request.Method == HttpMethod.Get &&
            UriConverter.ShouldUseOriginalString(request.Uri) &&
            request.IsDefaultVersionPolicy() &&
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
            writer.WriteMember(request, request.Method, "Method");
        }

        writer.WriteMember(request, request.Uri, "Uri");

        if (!request.IsDefaultVersion())
        {
            writer.WriteMember(request, request.Version, "Version");
        }

        if (!request.IsDefaultVersionPolicy())
        {
            writer.WriteMember(request, request.VersionPolicy, "VersionPolicy");
        }

        writer.WriteMember(request, request.Headers, "Headers");

        writer.WriteMember(request, request.ContentHeaders, "ContentHeaders");

        if (request.ContentStringParsed is string stringValue)
        {
            writer.WriteMember(request, stringValue, "ContentString");
        }
        else
        {
            writer.WriteMember(request, request.ContentStringParsed, "ContentStringParsed");
        }

        writer.WriteEndObject();
    }
}