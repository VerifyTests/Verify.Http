using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class PartialViewResultConverter :
    ResultConverter<PartialViewResult>
{
    protected override void InnerWrite(JsonWriter writer, PartialViewResult result, JsonSerializer serializer)
    {
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
        writer.WritePropertyName("ViewName");
        serializer.Serialize(writer, result.ViewName);
        writer.WritePropertyName("Model");
        serializer.Serialize(writer, result.Model);
    }
}