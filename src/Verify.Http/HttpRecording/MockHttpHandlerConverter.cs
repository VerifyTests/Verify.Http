using VerifyTests.Http;

class MockHttpHandlerConverter :
    WriteOnlyJsonConverter<MockHttpHandler>
{
    public override void Write(VerifyJsonWriter writer, MockHttpHandler handler)
    {
        writer.WriteStartObject();

        writer.WriteProperty(handler, handler.Calls, "Calls");

        writer.WriteEndObject();
    }
}