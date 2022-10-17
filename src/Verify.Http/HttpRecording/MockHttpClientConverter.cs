class MockHttpClientConverter :
    WriteOnlyJsonConverter<MockHttpClient>
{
    public override void Write(VerifyJsonWriter writer, MockHttpClient client)
    {
        writer.WriteStartObject();

        writer.WriteMember(client, client.Calls, "Calls");

        writer.WriteEndObject();
    }
}