using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class JsonResultConverter :
    ResultConverter<JsonResult>
{
    protected override void InnerWrite(JsonWriter writer, JsonResult result, JsonSerializer serializer)
    {
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("Value");
        serializer.Serialize(writer, result.Value);
        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
    }
}