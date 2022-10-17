#if NET5_0_OR_GREATER
using System.Net.Http.Headers;
using System.Net.Http.Json;
#endif

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

    #region ExplicitContent

    [Fact]
    public async Task ExplicitContent()
    {
        using var client = new MockHttpClient(
            content: @"{ ""a"": ""b"" }",
            mediaType: "application/json");

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region DefaultContent

    [Fact]
    public async Task DefaultContent()
    {
        using var client = new MockHttpClient();

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

#if NET6_0_OR_GREATER

    #region RecordedCalls

    [Fact]
    public async Task RecordedCalls()
    {
        using var client = new MockHttpClient();

        await client.GetAsync("https://fake/get1");
        await client.GetAsync("https://fake/get2");

        await Verify(client.Calls);
    }

    #endregion
#endif

    #region ExplicitStatusCode

    [Fact]
    public async Task ExplicitStatusCode()
    {
        using var client = new MockHttpClient(HttpStatusCode.Ambiguous);

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region ExplicitResponse

    [Fact]
    public async Task ExplicitResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Hello")
        };
        using var client = new MockHttpClient(response);

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region EnumerableResponses

    [Fact]
    public async Task EnumerableResponses()
    {
        using var client = new MockHttpClient(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Hello")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("World")
            });

        var result1 = await client.GetAsync("https://fake/get1");
        var result2 = await client.GetAsync("https://fake/get2");

        await Verify(new
        {
            result1,
            result2
        });
    }

    #endregion

    #region ResponseBuilder

    [Fact]
    public async Task ResponseBuilder()
    {
        using var client = new MockHttpClient(
            request =>
            {
                var content = $"Hello to {request.RequestUri}";
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content),
                };
                return response;
            });

        var result1 = await client.GetAsync("https://fake/get1");
        var result2 = await client.GetAsync("https://fake/get2");

        await Verify(new
        {
            result1,
            result2
        });
    }

    #endregion

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