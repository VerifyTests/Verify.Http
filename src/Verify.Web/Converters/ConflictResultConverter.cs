using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ConflictResultConverter :
    ResultConverter<ConflictResult>
{
    protected override void InnerWrite(JsonWriter writer, ConflictResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}