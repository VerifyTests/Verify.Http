using System.Net;
using VerifyTests.Http;

class HttpResponseConverter :
    WriteOnlyJsonConverter<HttpResponse>
{
    public override void Write(VerifyJsonWriter writer, HttpResponse response)
    {
        if (response.Status == HttpStatusCode.OK &&
            response.ContentHeaders == null &&
            response.Headers == null &&
            response.ContentStringRaw == null)
        {
            writer.WriteValue("200 Ok");
            return;
        }

        writer.WriteStartObject();

        writer.WriteProperty(response, $"{(int) response.Status} {response.Status}", "Status");

        writer.WriteProperty(response, response.Headers, "Headers");

        writer.WriteProperty(response, response.ContentHeaders, "ContentHeaders");

        writer.WriteProperty(response, response.ContentString, "ContentString");

        writer.WriteEndObject();
    }
}