using Newtonsoft.Json;
using VerifyTests;
using Microsoft.AspNetCore.Mvc;

class ControllerContextConverter :
    WriteOnlyJsonConverter<ControllerContext>
{
    public override void WriteJson(JsonWriter writer, ControllerContext? context, JsonSerializer serializer)
    {
        if (context == null)
        {
            return;
        }

        var response = context.HttpContext.Response;
        writer.WriteStartObject();

        HttpResponseConverter.WriteProperties(writer, serializer, response);

        writer.WriteEndObject();
    }

}