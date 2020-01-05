using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class NoContentResultConverter :
    ResultConverter<NoContentResult>
{
    protected override void InnerWrite(JsonWriter writer, NoContentResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}