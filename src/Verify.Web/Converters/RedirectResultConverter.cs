using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class RedirectResultConverter :
    ResultConverter<RedirectResult>
{
    protected override void InnerWrite(JsonWriter writer, RedirectResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("Url");
        serializer.Serialize(writer, result.Url);
        writer.WritePropertyName("PreserveMethod");
        serializer.Serialize(writer, result.PreserveMethod);
        writer.WritePropertyName("Permanent");
        serializer.Serialize(writer, result.Permanent);
    }
}