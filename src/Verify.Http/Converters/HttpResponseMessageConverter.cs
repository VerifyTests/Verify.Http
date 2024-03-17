class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void Write(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        writer.WriteStartObject();

        writer.WriteMember(response, response.Version, "Version");
        writer.WriteMember(response, response.StatusText(), "Status");

        WriteHeaders(writer, response);

        WriteCookies(writer, response);
        WriteTrailingHeaders(writer, response);
        writer.WriteMember(response, response.Content, "Content");

        writer.WriteEndObject();
    }

    static void WriteCookies(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var cookies = response.Headers.Cookies();
        writer.WriteMember(response, cookies, "Cookies");
    }

    static void WriteHeaders(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var headers = response.Headers.NotCookies();
        writer.WriteMember(response, headers, "Headers");
    }

    static void WriteTrailingHeaders(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var dictionary = response.TrailingHeaders.Simplify();
        if (dictionary.Count != 0)
        {
            writer.WriteMember(response, dictionary, "TrailingHeaders");
        }
    }
}