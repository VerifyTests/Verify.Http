class HttpResponseConverter :
    WriteOnlyJsonConverter<HttpResponse>
{
    public override void Write(VerifyJsonWriter writer, HttpResponse response)
    {
        if (response is
            {
                Status: HttpStatusCode.OK,
                ContentHeaders: null,
                Headers: null,
                ContentString: null
            })
        {
            writer.WriteValue("200 Ok");
            return;
        }

        writer.WriteStartObject();

        writer.WriteMember(response, $"{(int) response.Status} {response.Status}", "Status");

        writer.WriteMember(response, response.Headers, "Headers");

        writer.WriteMember(response, response.ContentHeaders, "ContentHeaders");

        if (response.ContentStringParsed is string stringValue)
        {
            writer.WriteMember(response, stringValue, "ContentString");
        }
        else
        {
            writer.WriteMember(response, response.ContentStringParsed, "ContentStringParsed");
        }

        writer.WriteEndObject();
    }
}