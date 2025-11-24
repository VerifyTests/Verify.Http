[TestFixture]
public class MockHttpClientTests
{
    [Test]
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

    [Test]
    public async Task ExplicitContent()
    {
        using var client = new MockHttpClient(
            content: """{ "a": "b" }""",
            mediaType: "application/json");

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region DefaultContent

    [Test]
    public async Task DefaultContent()
    {
        using var client = new MockHttpClient();

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region TrackedCalls

    [Test]
    public async Task TrackedCalls()
    {
        using var client = new MockHttpClient();

        await client.GetAsync("https://fake/get1");
        await client.GetAsync("https://fake/get2");

        await Verify(client.Calls);
    }

    #endregion

    #region ExplicitStatusCode

    [Test]
    public async Task ExplicitStatusCode()
    {
        using var client = new MockHttpClient(HttpStatusCode.Ambiguous);

        var result = await client.GetAsync("https://fake/get");

        await Verify(result);
    }

    #endregion

    #region ExplicitStatusCodes

    [Test]
    public async Task ExplicitStatusCodes()
    {
        using var client = new MockHttpClient(
            [
                HttpStatusCode.Ambiguous,
                HttpStatusCode.BadGateway,
                HttpStatusCode.GatewayTimeout,
                HttpStatusCode.InternalServerError
            ],
            recording: true);

        Recording.Start();

        await client.GetAsync("https://fake/get1");
        await client.GetAsync("https://fake/get2");
        await client.GetAsync("https://fake/get3");
        await client.GetAsync("https://fake/get4");

        await Verify();
    }

    #endregion

    #region ResponseFromFiles

    [Test]
    public async Task ResponseFromFiles()
    {
        using var client = new MockHttpClient(
            "sample.html",
            "sample.json",
            "sample.xml");

        using var content1 = await client.GetAsync("https://fake/get1");
        using var content2 = await client.GetAsync("https://fake/get2");
        using var content3 = await client.GetAsync("https://fake/get3");

        await Verify(new
            {
                content1,
                content2,
                content3
            })
            .IgnoreMember("Content-Length");
    }

    #endregion

    #region ExplicitResponse

    [Test]
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

    [Test]
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

        using var result1 = await client.GetAsync("https://fake/get1");
        using var result2 = await client.GetAsync("https://fake/get2");

        await Verify(new
        {
            result1,
            result2
        });
    }

    #endregion

    #region ResponseBuilder

    [Test]
    public async Task ResponseBuilder()
    {
        using var client = new MockHttpClient(request =>
        {
            var content = $"Hello to {request.RequestUri}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content),
            };
            return response;
        });

        using var result1 = await client.GetAsync("https://fake/get1");
        using var result2 = await client.GetAsync("https://fake/get2");

        await Verify(new
        {
            result1,
            result2
        });
    }

    #endregion

    [TestCase("application/json")]
    [TestCase("application/foo+json")]
    public async Task GetJsonContent(string mediaType)
    {
        using var client = new MockHttpClient(content: """{ "a": "b" }""", mediaType);

        var result = await client.GetAsync("https://fake/get");

        await Verify(result)
            .UseParameters(mediaType);
    }

    [Test]
    public async Task PostStringContent()
    {
        using var client = new MockHttpClient();

        using var content = new StringContent("the content");
        using var result = await client.PostAsync("https://fake/post", content);

        await Verify(
            new
            {
                result,
                client
            });
    }

    [TestCase("application/json")]
    [TestCase("application/foo+json")]
    public async Task PostJsonStringContent(string mediaType)
    {
        using var client = new MockHttpClient();

        using var content = new StringContent(
            content: """{ "a": "b" }""",
            encoding: Encoding.UTF8,
            mediaType: mediaType);

        using var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UseParameters(mediaType);
    }

    [TestCase("application/json")]
    [TestCase("application/foo+json")]
    public async Task PostJsonContent(string mediaType)
    {
        using var client = new MockHttpClient();

        using var content = JsonContent.Create(
            new
            {
                a = "b"
            },
            mediaType: MediaTypeHeaderValue.Parse(mediaType));
        using var result = await client.PostAsync("https://fake/post", content);

        await Verify(
                new
                {
                    result,
                    client
                })
            .UseParameters(mediaType);
    }

    [Test]
    public async Task PostStreamContent()
    {
        using var client = new MockHttpClient();

        await using var stream = new MemoryStream("the content"u8.ToArray());
        using var content = new StreamContent(stream);
        using var result = await client.PostAsync("https://fake/post", content);

        await Verify(
            new
            {
                result,
                client
            });
    }

    [Test]
    public async Task PostStreamContentWithContentType()
    {
        using var client = new MockHttpClient();

        await using var memoryStream = new MemoryStream("the content"u8.ToArray());
        using var streamContent = new StreamContent(memoryStream)
        {
            Headers =
            {
                ContentType = new("application/json")
            }
        };
        using var result = await client.PostAsync("https://fake/post", streamContent);

        await Verify(
            new
            {
                result,
                client
            });
    }

    [Test]
    public async Task PostFormContent()
    {
        using var client = new MockHttpClient();

        using var content = new FormUrlEncodedContent([new("a", "b"), new("c", "d")]);
        using var result = await client.PostAsync("https://fake/post", content);

        await Verify(
            new
            {
                result,
                client
            });
    }

    #region RecordingMockInteractions

    [Test]
    public async Task RecordingMockInteractions()
    {
        using var client = new MockHttpClient(recording: true);

        Recording.Start();
        await client.GetStringAsync("https://fake/getOne");
        await client.GetStringAsync("https://fake/getTwo");

        await Verify();
    }

    #endregion

    [Test]
    public async Task ResponseHeadersRead_StreamIsNotSeekable()
    {
        using var client = new MockHttpClient();

        var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() => stream.Position = 1);
    }

    [Test]
    public async Task ResponseHeadersRead_LengthIsNotAvaliable()
    {
        using var client = new MockHttpClient();

        var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once UnusedVariable
            var x = stream.Length;
        });
    }

    [Test]
    public async Task ResponseHeadersRead_WithFile_StreamIsNotSeekable()
    {
        using var client = new MockHttpClient("sample.html")
        {
            SimulateNetworkStream = true
        };

        var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() => stream.Position = 1);
    }

    [Test]
    public async Task ResponseHeadersRead_WithFile_LengthIsNotAvaliable()
    {
        using var client = new MockHttpClient("sample.html")
        {
            SimulateNetworkStream = true
        };

        var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once UnusedVariable
            var x = stream.Length;
        });
    }
}