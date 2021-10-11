using System.Net.Http.Headers;
using Newtonsoft.Json;
using VerifyTests;

class HttpHeadersConverter :
    WriteOnlyJsonConverter<HttpHeaders>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpHeaders headers,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        serializer.Serialize(writer, headers.Simplify());
    }
}