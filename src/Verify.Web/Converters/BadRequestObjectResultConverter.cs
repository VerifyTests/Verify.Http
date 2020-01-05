using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class BadRequestObjectResultConverter :
    ResultConverter<BadRequestObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, BadRequestObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}