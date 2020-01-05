using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class OkResultConverter :
    ResultConverter<OkResult>
{
    protected override void InnerWrite(JsonWriter writer, OkResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}