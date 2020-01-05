using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class PhysicalFileResultConverter :
    ResultConverter<PhysicalFileResult>
{
    protected override void InnerWrite(JsonWriter writer, PhysicalFileResult result, JsonSerializer serializer)
    {
        FileResultConverter.WriteFileData(writer, result, serializer);
        writer.WritePropertyName("FileName");
        serializer.Serialize(writer, result.FileName);
    }
}