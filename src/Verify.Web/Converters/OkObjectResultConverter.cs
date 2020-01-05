using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class OkObjectResultConverter :
    ResultConverter<OkObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, OkObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}