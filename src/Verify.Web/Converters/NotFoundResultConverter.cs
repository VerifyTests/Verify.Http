using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class NotFoundResultConverter :
    ResultConverter<NotFoundResult>
{
    protected override void InnerWrite(JsonWriter writer, NotFoundResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}