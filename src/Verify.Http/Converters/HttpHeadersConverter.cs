using System.Net.Http.Headers;

class HttpHeadersConverter :
    WriteOnlyJsonConverter<HttpHeaders>
{
    public override void Write(VerifyJsonWriter writer, HttpHeaders headers) =>
        writer.Serialize(headers.Simplify());
}