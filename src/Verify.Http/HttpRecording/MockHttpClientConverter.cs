using Newtonsoft.Json;
using VerifyTests.Http;

class MockHttpClientConverter :
    WriteOnlyJsonConverter<MockHttpClient>
{
    public override void Write(
        VerifyJsonWriter writer,
        MockHttpClient client,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Calls");
        serializer.Serialize(writer, client.Calls);

        writer.WriteEndObject();
    }
}