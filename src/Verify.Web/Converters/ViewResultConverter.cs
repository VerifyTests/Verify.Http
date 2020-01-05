using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ViewResultConverter :
    ResultConverter<ViewResult>
{
    protected override void InnerWrite(JsonWriter writer, ViewResult result, JsonSerializer serializer)
    {
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
        writer.WritePropertyName("Model");
        serializer.Serialize(writer, result.Model);
        if (result.ViewData.Any())
        {
            writer.WritePropertyName("ViewData");
            serializer.Serialize(writer, result.ViewData.ToDictionary(x => x.Key, x => x.Value));
        }

        if (result.TempData.Any())
        {
            writer.WritePropertyName("TempData");
            serializer.Serialize(writer, result.TempData.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}