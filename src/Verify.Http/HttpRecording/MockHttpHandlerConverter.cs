using Newtonsoft.Json;
using VerifyTests.Http;

class MockHttpHandlerConverter :
    WriteOnlyJsonConverter<MockHttpHandler>
{
    public override void WriteJson(
        JsonWriter writer,
        MockHttpHandler handler,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Calls");
        serializer.Serialize(writer, handler.Calls);

        writer.WriteEndObject();
    }
}