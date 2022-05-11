#if NET5_0_OR_GREATER
using System.Net.Http.Headers;
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

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/foo+json")]
    public async Task GetJsonContent(string mediaType)
    {
        using var client = new MockHttpClient(content: @"{ ""a"": ""b"" }", mediaType);

        var result = await client.GetAsync("https://fake/get");

        await Verify(result)
            .UseParameters(mediaType)
            .UniqueForRuntimeAndVersion();
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

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/foo+json")]
    public async Task PostJsonStringContent(string mediaType)
    {
        using var client = new MockHttpClient();

        var content = new StringContent(
            content: @"{ ""a"": ""b"" }",
            encoding: Encoding.UTF8,
            mediaType: mediaType);

        var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UseParameters(mediaType)
            .UniqueForRuntimeAndVersion();
    }

#if NET5_0_OR_GREATER
    [Theory]
    [InlineData("application/json")]
    [InlineData("application/foo+json")]
    public async Task PostJsonContent(string mediaType)
    {
        using var client = new MockHttpClient();

        var content = JsonContent.Create(
            new
            {
                a = "b"
            },
            mediaType: MediaTypeHeaderValue.Parse(mediaType));
        var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UseParameters(mediaType)
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