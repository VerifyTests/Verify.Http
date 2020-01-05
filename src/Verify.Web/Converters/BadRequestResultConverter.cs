using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class BadRequestResultConverter :
    ResultConverter<BadRequestResult>
{
    protected override void InnerWrite(JsonWriter writer, BadRequestResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}