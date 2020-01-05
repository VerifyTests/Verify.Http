using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ObjectResultConverter :
    ResultConverter<ObjectResult>
{
    protected override void InnerWrite(JsonWriter writer, ObjectResult result, JsonSerializer serializer)
    {
        Write(writer, result, serializer);
    }

    public static void Write(JsonWriter writer, ObjectResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("Value");
        serializer.Serialize(writer, result.Value);
        if (result.StatusCode != null)
        {
            writer.WritePropertyName("StatusCode");
            serializer.Serialize(writer, result.StatusCode);
        }

        writer.WritePropertyName("ContentTypes");
        serializer.Serialize(writer, result.ContentTypes);
        writer.WritePropertyName("DeclaredType");
        serializer.Serialize(writer, result.DeclaredType.FullName);
    }
}