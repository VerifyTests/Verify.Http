// ReSharper disable UnusedVariable
public class Tests
{
    #region IgnoreHeader

    [Fact]
    public async Task IgnoreHeader()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

        await Verify(result)
            .IgnoreMembers("Server");
    }

    #endregion

    [Fact]
    public async Task JsonGet()
    {
        Recording.Start();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://github.com/VerifyTests/Verify.Http/raw/main/src/global.json");

        await VerifyJson(result);
    }

    [Fact]
    public async Task TestHttpRecordingWithResponse()
    {
        Recording.Start();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

        await Verify(result);
    }

    #region ServiceThatDoesHttp

    // Resolve a HttpClient. All http calls done at any
    // resolved client will be added to `recording.Sends`
    public class MyService(HttpClient client)
    {
        public Task MethodThatDoesHttp() =>
            // Some code that does some http calls
            client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");
    }

    #endregion

    [Fact]
    public async Task MediaTypePlainTextIsRecorded()
    {
        const string content = "string content 123";
        var recordingHandler = new RecordingHandler();
        recordingHandler.InnerHandler = new ContentHandler(new(content, Encoding.UTF8));
        using var client = new HttpClient(recordingHandler);

        var response = await client.GetAsync("https://dont-care.org/get");

        Assert.Equal(content, recordingHandler.Sends.Single().ResponseContent);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/foo+json")]
    public async Task MediaTypeApplicationJsonIsRecorded(string mediaType)
    {
        const string content = "{ \"age\": 1234 }";
        var recordingHandler = new RecordingHandler();
        recordingHandler.InnerHandler = new ContentHandler(new(content, Encoding.UTF8, mediaType));
        using var client = new HttpClient(recordingHandler);

        var response = await client.GetAsync("https://dont-care.org/get");

        Assert.Equal(content, recordingHandler.Sends.Single().ResponseContent);
    }
#if DEBUG

    #region HttpRecording

    [Fact]
    public async Task TestHttpRecording()
    {
        Recording.Start();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        await Verify(
                new
                {
                    sizeOfResponse
                });
    }

    static async Task<int> MethodThatDoesHttpCalls()
    {
        using var client = new HttpClient();

        var jsonResult = await client.GetStringAsync("https://github.com/VerifyTests/Verify.Http/raw/main/src/global.json");
        var ymlResult = await client.GetStringAsync("https://github.com/VerifyTests/Verify.Http/raw/main/src/appveyor.yml");
        return jsonResult.Length + ymlResult.Length;
    }

    #endregion


    #region HttpRecordingExplicit

    [Fact]
    public async Task TestHttpRecordingExplicit()
    {
        Recording.Start();

        var responseSize = await MethodThatDoesHttpCalls();

        var httpCalls = Recording.Stop()
            .Select(_ => _.Data)
            .OfType<HttpCall>()
            .ToList();

        // Ensure all calls finished in under 5 seconds
        var threshold = TimeSpan.FromSeconds(5);
        foreach (var call in httpCalls)
        {
            Assert.True(call.Duration < threshold);
        }

        await Verify(
            new
            {
                responseSize,
                // Only use the Uri in the snapshot
                httpCalls = httpCalls.Select(_ => _.Request.Uri)
            });
    }

    #endregion
#endif

    [Fact]
    public async Task HttpClientRecordingGlobal()
    {
        #region HttpClientRecordingGlobal

        var collection = new ServiceCollection();
        collection.AddScoped<MyService>();

        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var (builder, recording) = collection.AddRecordingHttpClient();

        await using var provider = collection.BuildServiceProvider();

        var myService = provider.GetRequiredService<MyService>();

        await myService.MethodThatDoesHttp();

        await Verify(recording.Sends);

        #endregion
    }

    [Fact]
    public async Task HttpClientRecording()
    {
        #region HttpClientRecording

        var collection = new ServiceCollection();
        collection.AddScoped<MyService>();
        var httpBuilder = collection.AddHttpClient<MyService>();

        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var recording = httpBuilder.AddRecording();

        await using var provider = collection.BuildServiceProvider();

        var myService = provider.GetRequiredService<MyService>();

        await myService.MethodThatDoesHttp();

        await Verify(recording.Sends);

        #endregion
    }

    [Fact]
    public async Task HttpResponseNested()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

        await Verify(
            new
            {
                result
            });
    }

    [Fact]
    public Task Uri() =>
        Verify(
            new
            {
                uri1 = new Uri("http://127.0.0.1:57754/admin/databases"),
                uri2 = new Uri("http://127.0.0.1:57754/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483"),
                uri3 = new Uri("http://127.0.0.1/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483"),
                uri4 = new Uri("http://127.0.0.1/?name"),
                uri5 = new Uri("http://127.0.0.1/?name="),
                uri6 = new Uri("/admin/databases", UriKind.Relative),
                uri7 = new Uri("/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483", UriKind.Relative),
                uri8 = new Uri("/admin/databases?name=HttpRecordingTest&replicationFactor=1&raft-request-id=1331f44c-02de-4d00-a645-28bc1b639483", UriKind.Relative),
                uri9 = new Uri("/?name", UriKind.Relative),
                uri10 = new Uri("/?name=", UriKind.Relative)
            });

    [Fact]
    public async Task ImageHttpResponse()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/src/icon.png");

        await Verify(result);
    }

    #region HttpResponse

    [Fact]
    public async Task HttpResponse()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

        await Verify(result);
    }

    #endregion

    [Fact]
    public Task HttpStatusCodeTest() =>
        Verify(HttpStatusCode.Ambiguous)
            .UniqueForRuntime();

    [Fact]
    public async Task PauseResume()
    {
        #region HttpClientPauseResume

        var collection = new ServiceCollection();
        collection.AddScoped<MyService>();
        var httpBuilder = collection.AddHttpClient<MyService>();

        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var recording = httpBuilder.AddRecording();

        await using var provider = collection.BuildServiceProvider();

        var myService = provider.GetRequiredService<MyService>();

        // Recording is enabled by default. So Pause to stop recording
        recording.Pause();
        await myService.MethodThatDoesHttp();

        // Resume recording
        recording.Resume();
        await myService.MethodThatDoesHttp();

        await Verify(recording.Sends);

        #endregion
    }

    [Fact]
    public async Task RecordingFullControl()
    {
        #region HttpClientRecordingExplicit

        var collection = new ServiceCollection();

        var builder = collection.AddHttpClient("name");

        // Change to not recording at startup
        var recording = new RecordingHandler(recording: false);

        builder.AddHttpMessageHandler(() => recording);

        await using var provider = collection.BuildServiceProvider();

        var factory = provider.GetRequiredService<IHttpClientFactory>();

        var client = factory.CreateClient("name");

        await client.GetAsync("https://www.google.com/");

        recording.Resume();
        await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

        await Verify(recording.Sends);

        #endregion
    }
}