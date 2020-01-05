using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class CreatedAtRouteResultConverter :
    ResultConverter<CreatedAtRouteResult>
{
    protected override void InnerWrite(JsonWriter writer, CreatedAtRouteResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("RouteName");
        serializer.Serialize(writer, result.RouteName);
        if (result.RouteValues.Any())
        {
            writer.WritePropertyName("RouteValues");
            serializer.Serialize(writer, result.RouteValues.ToDictionary(x => x.Key, x => x.Value));
        }

        ObjectResultConverter.Write(writer, result, serializer);
    }
}