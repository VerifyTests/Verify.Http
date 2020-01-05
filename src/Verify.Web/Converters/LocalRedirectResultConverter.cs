using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class LocalRedirectResultConverter :
    ResultConverter<LocalRedirectResult>
{
    protected override void InnerWrite(JsonWriter writer, LocalRedirectResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("PreserveMethod");
        serializer.Serialize(writer, result.PreserveMethod);
        writer.WritePropertyName("Permanent");
        serializer.Serialize(writer, result.Permanent);
        writer.WritePropertyName("Url");
        serializer.Serialize(writer, result.Url);
    }
}