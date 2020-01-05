using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class VirtualFileResultConverter :
    ResultConverter<VirtualFileResult>
{
    protected override void InnerWrite(JsonWriter writer, VirtualFileResult result, JsonSerializer serializer)
    {
        FileResultConverter.WriteFileData(writer, result, serializer);
        writer.WritePropertyName("FileName");
        serializer.Serialize(writer, result.FileName);
    }
}