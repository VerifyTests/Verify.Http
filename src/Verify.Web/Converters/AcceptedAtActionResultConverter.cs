using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class AcceptedAtActionResultConverter :
    ResultConverter<AcceptedAtActionResult>
{
    protected override void InnerWrite(JsonWriter writer, AcceptedAtActionResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("ActionName");
        serializer.Serialize(writer, result.ActionName);
        writer.WritePropertyName("ControllerName");
        serializer.Serialize(writer, result.ControllerName);
        if (result.RouteValues.Any())
        {
            writer.WritePropertyName("RouteValues");
            serializer.Serialize(writer, result.RouteValues.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}