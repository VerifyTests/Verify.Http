using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Verify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

        WriteHeaders(writer, serializer, response);

        WriteCookies(writer, serializer, response);

        writer.WriteEndObject();
    }

    static void WriteCookies(JsonWriter writer, JsonSerializer serializer, HttpResponse response)
    {
        var cookies = response.Headers
            .Where(x => x.Key == HeaderNames.SetCookie)
            .Select(x =>
            {
                var stringSegment = x.Value.Single();
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(x=>x.Name.Value,x=>x.Value.Value);
        if (!cookies.Any())
        {
            return;
        }

        writer.WritePropertyName("Cookies");
        serializer.Serialize(writer, cookies);
    }

    static void WriteHeaders(JsonWriter writer, JsonSerializer serializer, HttpResponse response)
    {
        var headers = response.Headers
            .Where(x => x.Key != HeaderNames.SetCookie)
            .ToDictionary(x => x.Key, x => x.Value.ToString());
        if (!headers.Any())
        {
            return;
        }

        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, headers);
    }
}