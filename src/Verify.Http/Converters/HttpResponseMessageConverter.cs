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
#if NET5_0_OR_GREATER || NETSTANDARD2_1
        WriteTrailingHeaders(writer, response);
#endif
        writer.WriteMember(response, response.Content, "Content");
        writer.WriteMember(response, response.RequestMessage, "Request");

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

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    static void WriteTrailingHeaders(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var dictionary = response.TrailingHeaders.Simplify();
        if (dictionary.Any())
        {
            writer.WriteMember(response, dictionary, "TrailingHeaders");
        }
    }
#endif
}