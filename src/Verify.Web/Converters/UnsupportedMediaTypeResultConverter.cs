using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class UnsupportedMediaTypeResultConverter :
    ResultConverter<UnsupportedMediaTypeResult>
{
    protected override void InnerWrite(JsonWriter writer, UnsupportedMediaTypeResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}