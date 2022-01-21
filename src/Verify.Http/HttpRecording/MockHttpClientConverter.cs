using VerifyTests.Http;

class MockHttpClientConverter :
    WriteOnlyJsonConverter<MockHttpClient>
{
    public override void Write(VerifyJsonWriter writer, MockHttpClient client)
    {
        writer.WriteStartObject();

        writer.WriteProperty(client, client.Calls, "Calls");

        writer.WriteEndObject();
    }
}