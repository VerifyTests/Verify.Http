using System.Net;
using Newtonsoft.Json;
using VerifyTests.Http;

class HttpResponseConverter :
    WriteOnlyJsonConverter<HttpResponse>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpResponse response,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (response.Status == HttpStatusCode.OK &&
            response.ContentHeaders == null &&
            response.Headers == null &&
            response.ContentStringRaw == null)
        {
            writer.WriteValue("200 Ok");
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("Status");
        writer.WriteValue($"{(int) response.Status} {response.Status}");

        if (response.Headers != null)
        {
            writer.WritePropertyName("Headers");
            serializer.Serialize(writer, response.Headers);
        }

        if (response.ContentHeaders != null)
        {
            writer.WritePropertyName("ContentHeaders");
            serializer.Serialize(writer, response.ContentHeaders);
        }

        if (response.ContentString != null)
        {
            writer.WritePropertyName("ContentString");
            serializer.Serialize(writer, response.ContentString);
        }

        writer.WriteEndObject();
    }
}