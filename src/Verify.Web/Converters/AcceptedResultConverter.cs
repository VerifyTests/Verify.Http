using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class AcceptedResultConverter :
    ResultConverter<AcceptedResult>
{
    protected override void InnerWrite(JsonWriter writer, AcceptedResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("Location");
        serializer.Serialize(writer, result.Location);
        ObjectResultConverter.Write(writer, result, serializer);
    }
}