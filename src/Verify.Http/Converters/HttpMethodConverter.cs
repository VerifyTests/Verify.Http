class HttpMethodConverter :
    WriteOnlyJsonConverter<HttpMethod>
{
    public override void Write(VerifyJsonWriter writer, HttpMethod method)
    {
        writer.WriteValue(method.ToString());
    }
}