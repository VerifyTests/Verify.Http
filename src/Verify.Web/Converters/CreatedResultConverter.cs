using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class CreatedResultConverter :
    ResultConverter<CreatedResult>
{
    protected override void InnerWrite(JsonWriter writer, CreatedResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("Location");
        serializer.Serialize(writer, result.Location);
        ObjectResultConverter.Write(writer, result, serializer);
    }
}