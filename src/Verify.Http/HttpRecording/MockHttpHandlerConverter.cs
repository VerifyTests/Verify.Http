using Newtonsoft.Json;
using VerifyTests.Http;

class MockHttpHandlerConverter :
    WriteOnlyJsonConverter<MockHttpHandler>
{
    public override void Write(
        VerifyJsonWriter writer,
        MockHttpHandler handler,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Calls");
        serializer.Serialize(writer, handler.Calls);

        writer.WriteEndObject();
    }
}