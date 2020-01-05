using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class NotFoundObjectResultConverter :
    ResultConverter<NotFoundObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, NotFoundObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}