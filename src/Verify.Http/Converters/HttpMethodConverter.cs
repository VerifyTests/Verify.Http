using Newtonsoft.Json;

class HttpMethodConverter :
    WriteOnlyJsonConverter<HttpMethod>
{
    public override void Write(
        VerifyJsonWriter writer,
        HttpMethod method,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, method.ToString());
    }
}