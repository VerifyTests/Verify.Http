using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

static class FileResultConverter
{
    public static void WriteFileData(JsonWriter writer, FileResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("FileDownloadName");
        serializer.Serialize(writer, result.FileDownloadName);

        if (result.LastModified != null)
        {
            writer.WritePropertyName("LastModified");
            serializer.Serialize(writer, result.LastModified);
        }

        writer.WritePropertyName("EntityTag");
        serializer.Serialize(writer, result.EntityTag);
        writer.WritePropertyName("EnableRangeProcessing");
        serializer.Serialize(writer, result.EnableRangeProcessing);
        writer.WritePropertyName("ContentType");
        serializer.Serialize(writer, result.ContentType);
    }
}