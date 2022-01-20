using System.Net.Http.Headers;
using Newtonsoft.Json;

class HttpHeadersConverter :
    WriteOnlyJsonConverter<HttpHeaders>
{
    public override void Write(
        VerifyJsonWriter writer,
        HttpHeaders headers,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, headers.Simplify());
    }
}