using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class FileStreamResultConverter :
    ResultConverter<FileStreamResult>
{
    protected override void InnerWrite(JsonWriter writer, FileStreamResult result, JsonSerializer serializer)
    {
        //TODO: do stream
        FileResultConverter.WriteFileData(writer, result, serializer);
    }
}