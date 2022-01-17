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
        writer.WriteStartObject();

        writer.WritePropertyName("Method");
        serializer.Serialize(writer, request.Method);

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