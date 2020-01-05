using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ContentResultConverter :
    ResultConverter<ContentResult>
{
    protected override void InnerWrite(JsonWriter writer, ContentResult result, JsonSerializer serializer)
    {
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("Content");
        serializer.Serialize(writer, result.Content);
        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
    }
}