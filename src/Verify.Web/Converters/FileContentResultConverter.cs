using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class FileContentResultConverter :
    ResultConverter<FileContentResult>
{
    protected override void InnerWrite(JsonWriter writer, FileContentResult result, JsonSerializer serializer)
    {
        FileResultConverter.WriteFileData(writer, result, serializer);
    }
}