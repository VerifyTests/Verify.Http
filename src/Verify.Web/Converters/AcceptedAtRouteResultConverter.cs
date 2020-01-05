using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class AcceptedAtRouteResultConverter :
    ResultConverter<AcceptedAtRouteResult>
{
    protected override void InnerWrite(JsonWriter writer, AcceptedAtRouteResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("RouteName");
        serializer.Serialize(writer, result.RouteName);
        if (result.RouteValues.Any())
        {
            writer.WritePropertyName("RouteValues");
            serializer.Serialize(writer, result.RouteValues.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}