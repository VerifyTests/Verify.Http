using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ViewComponentResultConverter :
    ResultConverter<ViewComponentResult>
{
    protected override void InnerWrite(JsonWriter writer, ViewComponentResult result, JsonSerializer serializer)
    {
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
        writer.WritePropertyName("ViewComponentName");
        serializer.Serialize(writer, result.ViewComponentName);
        writer.WritePropertyName("ViewComponentType");
        serializer.Serialize(writer, result.ViewComponentType.FullName);
        if (result.TempData.Any())
        {
            writer.WritePropertyName("TempData");
            serializer.Serialize(writer, result.TempData.ToDictionary(x => x.Key, x => x.Value));
        }

        if (result.ViewData.Any())
        {
            writer.WritePropertyName("ViewData");
            serializer.Serialize(writer, result.ViewData.ToDictionary(x => x.Key, x => x.Value));
        }

        writer.WritePropertyName("Arguments");
        serializer.Serialize(writer, result.Arguments);
        writer.WritePropertyName("Model");
        serializer.Serialize(writer, result.Model);
    }
}