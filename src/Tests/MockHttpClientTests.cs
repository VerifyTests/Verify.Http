#if NET5_0_OR_GREATER
using System.Net.Http.Json;
#endif
using VerifyTests.Http;

[UsesVerify]
public class MockHttpClientTests
{
    [Fact]
    public async Task Get()
    {
        using var client = new MockHttpClient();

        var result = await client.GetStringAsync("https://fake/get");

        await Verify(
            new
            {
                result,
                client
            });
    }

    [Fact]
    public async Task PostStringContent()
    {
        using var client = new MockHttpClient();

        var content = new StringContent("the content");
        var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UniqueForRuntimeAndVersion();
    }

#if NET5_0_OR_GREATER
    [Fact]
    public async Task PostJsonContent()
    {
        using var client = new MockHttpClient();

        var content = JsonContent.Create(
            new
            {
                a = "b"
            });
        var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UniqueForRuntimeAndVersion();
    }
#endif

    [Fact]
    public async Task PostStreamContent()
    {
        using var client = new MockHttpClient();

        var stream = new MemoryStream(Encoding.UTF8.GetBytes("the content"));
        var content = new StreamContent(stream);
        var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UniqueForRuntimeAndVersion();
    }

    [Fact]
    public async Task PostStreamContentWithContentType()
    {
        using var client = new MockHttpClient();

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("the content"));
        var streamContent = new StreamContent(memoryStream)
        {
            Headers =
            {
                ContentType = new("application/json")
            }
        };
        var result = await client.PostAsync("https://fake/post", streamContent);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UniqueForRuntimeAndVersion();
    }
}