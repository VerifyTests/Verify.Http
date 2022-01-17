using Newtonsoft.Json;
using VerifyTests.Http;

class HttpRequestConverter :
    WriteOnlyJsonConverter<HttpRequest>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpRequest request,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (request.ContentHeaders == null &&
            request.Headers == null &&
            request.ContentStringRaw == null)
        {
            serializer.Serialize(writer, request.Uri);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("Uri");
        serializer.Serialize(writer, request.Uri);

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