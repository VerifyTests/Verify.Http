using Newtonsoft.Json;
using VerifyTests.Http;

class HttpRequestConverter :
    WriteOnlyJsonConverter<HttpRequest>
{
    public override void Write(VerifyJsonWriter writer, HttpRequest request, JsonSerializer serializer)
    {
        if (request.Method == HttpMethod.Get &&
            UriConverter.ShouldUseOriginalString(request.Uri) &&
#if NET5_0_OR_GREATER
            request.IsDefaultVersionPolicy() &&
#endif
            request.Headers == null &&
            request.ContentHeaders == null &&
            request.ContentString == null
            )
        {
            writer.WriteValue(request.Uri.OriginalString);
            return;
        }

        writer.WriteStartObject();

        if (request.Method != HttpMethod.Get)
        {
            writer.WritePropertyName("Method");
            serializer.Serialize(writer, request.Method);
        }

        writer.WritePropertyName("Uri");
        serializer.Serialize(writer, request.Uri);

        if (!request.IsDefaultVersion())
        {
            writer.WritePropertyName("Version");
            serializer.Serialize(writer, request.Version);
        }

#if NET5_0_OR_GREATER

        if (!request.IsDefaultVersionPolicy())
        {
            writer.WritePropertyName("VersionPolicy");
            serializer.Serialize(writer, request.VersionPolicy);
        }

#endif

        if (request.Headers != null)
        {
            writer.WritePropertyName("Headers");
            serializer.Serialize(writer, request.Headers);
        }

        if (request.ContentHeaders != null)
        {
            writer.WritePropertyName("ContentHeaders");
            serializer.Serialize(writer, request.ContentHeaders);
        }

        if (request.ContentString != null)
        {
            writer.WritePropertyName("ContentString");
            serializer.Serialize(writer, request.ContentString);
        }

        writer.WriteEndObject();
    }
}