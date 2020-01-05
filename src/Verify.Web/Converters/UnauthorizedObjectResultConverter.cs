using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class UnauthorizedObjectResultConverter :
    ResultConverter<UnauthorizedObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, UnauthorizedObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}