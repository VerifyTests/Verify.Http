using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class UnauthorizedResultConverter :
    ResultConverter<UnauthorizedResult>
{
    protected override void InnerWrite(JsonWriter writer, UnauthorizedResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("StatusCode");
        serializer.Serialize(writer, result.StatusCode);
    }
}