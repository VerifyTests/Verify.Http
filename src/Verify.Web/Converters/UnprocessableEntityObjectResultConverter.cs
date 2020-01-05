using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class UnprocessableEntityObjectResultConverter :
    ResultConverter<UnprocessableEntityObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, UnprocessableEntityObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}