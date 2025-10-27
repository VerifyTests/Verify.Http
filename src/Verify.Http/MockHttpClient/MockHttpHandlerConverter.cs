class MockHttpHandlerConverter :
    WriteOnlyJsonConverter<MockHttpHandler>
{
    public override void Write(VerifyJsonWriter writer, MockHttpHandler handler)
    {
        writer.WriteStartObject();

        writer.WriteMember(handler, handler.Calls, "Calls");

        writer.WriteEndObject();
    }
}