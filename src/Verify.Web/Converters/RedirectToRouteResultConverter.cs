using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class RedirectToRouteResultConverter :
    ResultConverter<RedirectToRouteResult>
{
    protected override void InnerWrite(JsonWriter writer, RedirectToRouteResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("Fragment");
        serializer.Serialize(writer, result.Fragment);
        writer.WritePropertyName("Permanent");
        serializer.Serialize(writer, result.Permanent);
        writer.WritePropertyName("PreserveMethod");
        serializer.Serialize(writer, result.PreserveMethod);
        writer.WritePropertyName("RouteName");
        serializer.Serialize(writer, result.RouteName);
        if (result.RouteValues.Any())
        {
            writer.WritePropertyName("RouteValues");
            serializer.Serialize(writer, result.RouteValues.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}