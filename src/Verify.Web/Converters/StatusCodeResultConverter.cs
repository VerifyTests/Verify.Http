using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class StatusCodeResultConverter :
    ResultConverter<StatusCodeResult>
{
    protected override void InnerWrite(JsonWriter writer, StatusCodeResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}