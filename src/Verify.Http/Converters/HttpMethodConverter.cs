using Newtonsoft.Json;

class HttpMethodConverter :
    WriteOnlyJsonConverter<HttpMethod>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpMethod method,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        serializer.Serialize(writer, method.ToString());
    }
}