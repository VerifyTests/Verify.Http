using Newtonsoft.Json;
using VerifyTests.Http;

class MockHttpClientConverter :
    WriteOnlyJsonConverter<MockHttpClient>
{
    public override void WriteJson(
        JsonWriter writer,
        MockHttpClient client,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Calls");
        serializer.Serialize(writer, client.Calls);

        writer.WriteEndObject();
    }
}