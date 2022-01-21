class HttpResponseMessageConverter :
    WriteOnlyJsonConverter<HttpResponseMessage>
{
    public override void Write(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        writer.WriteStartObject();

        writer.WriteProperty(response, response.Version, "Version");
        writer.WriteProperty(response, response.StatusText(), "Status");

        WriteHeaders(writer, response);

        WriteCookies(writer, response);
#if NET5_0_OR_GREATER || NETSTANDARD2_1
        WriteTrailingHeaders(writer, response);
#endif
        writer.WriteProperty(response, response.Content, "Content");
        writer.WriteProperty(response, response.RequestMessage, "Request");

        writer.WriteEndObject();
    }

    static void WriteCookies(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var cookies = response.Headers.Cookies();
        writer.WriteProperty(response, cookies, "Cookies");
    }

    static void WriteHeaders(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        var headers = response.Headers.NotCookies();
        writer.WriteProperty(response, headers, "Headers");
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    static void WriteTrailingHeaders(VerifyJsonWriter writer, HttpResponseMessage response)
    {
        writer.WriteProperty(response, response.TrailingHeaders, "TrailingHeaders");
    }
#endif
}