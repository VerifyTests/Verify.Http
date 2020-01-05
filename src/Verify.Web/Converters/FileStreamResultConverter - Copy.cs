using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class PhysicalFileResultConverter :
    ResultConverter<PhysicalFileResult>
{
    protected override void InnerWrite(JsonWriter writer, PhysicalFileResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("FileName");
        serializer.Serialize(writer, result.FileName);
        FileResultConverter.WriteFileData(writer, result, serializer);
    }
}