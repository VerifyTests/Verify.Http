using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class UnprocessableEntityResultConverter :
    ResultConverter<UnprocessableEntityResult>
{
    protected override void InnerWrite(JsonWriter writer, UnprocessableEntityResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}