class HttpContentHeadersConverter :
    WriteOnlyJsonConverter<HttpContentHeaders>
{
    public override void Write(VerifyJsonWriter writer, HttpContentHeaders headers) =>
        writer.Serialize(headers.Simplify());
}