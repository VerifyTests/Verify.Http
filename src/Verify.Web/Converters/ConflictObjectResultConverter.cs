using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ConflictObjectResultConverter :
    ResultConverter<ConflictObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, ConflictObjectResult result, JsonSerializer serializer)
    {
        ObjectResultConverter.Write(writer, result, serializer);
    }
}