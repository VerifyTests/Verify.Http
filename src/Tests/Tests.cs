using Microsoft.Extensions.DependencyInjection;
using VerifyTests.Http;

[UsesVerify]
public class Tests
{
#if NET5_0_OR_GREATER && DEBUG
    [Fact]
    public async Task JsonGet()
    {
        HttpRecording.StartRecording();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://httpbin.org/get");

        await Verify(result);
    }

    [Fact]
    public async Task TestHttpRecordingWithResponse()
    {
        HttpRecording.StartRecording();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://httpbin.org/json");

        await Verify(result);
    }
#endif

    #region ServiceThatDoesHttp

    public class MyService
    {
        HttpClient client;

        // Resolve a HttpClient. All http calls done at any
        // resolved client will be added to `recording.Sends`
        public MyService(HttpClient client) =>
            this.client = client;

        public Task MethodThatDoesHttp() =>
            // Some code that does some http calls
            client.GetAsync("https://httpbin.org/status/undefined");
    }

    #endregion

#if(NET5_0_OR_GREATER)

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

    #region HttpRecording

    [Fact]
    public async Task TestHttpRecording()
    {
        HttpRecording.StartRecording();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        await Verify(
                new
                {
                    sizeOfResponse
                })
            .ModifySerialization(settings =>
            {
                //scrub some headers that are not consistent between test runs
                settings.IgnoreMembers("traceparent", "Date");
            });
    }

    static async Task<int> MethodThatDoesHttpCalls()
    {
        using var client = new HttpClient();

        var jsonResult = await client.GetStringAsync("https://httpbin.org/json");
        var xmlResult = await client.GetStringAsync("https://httpbin.org/xml");
        return jsonResult.Length + xmlResult.Length;
    }

    #endregion

    #region HttpRecordingExplicit

    [Fact]
    public async Task TestHttpRecordingExplicit()
    {
        HttpRecording.StartRecording();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        var httpCalls = HttpRecording.FinishRecording().ToList();

        // Ensure all calls finished in under 5 seconds
        var threshold = TimeSpan.FromSeconds(5);
        foreach (var call in httpCalls)
        {
            Assert.True(call.Duration < threshold);
        }

        await Verify(
            new
            {
                sizeOfResponse,
                // Only use the Uri in the snapshot
                httpCalls = httpCalls.Select(_ => _.Request.Uri)
            });
    }

    #endregion

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

        await Verify(recording.Sends)
            // Ignore some headers that change per request
            .ModifySerialization(x => x.IgnoreMembers("Date"));

        #endregion
    }

#endif

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

        await Verify(recording.Sends)
            // Ignore some headers that change per request
            .ModifySerialization(x => x.IgnoreMembers("Date"));

        #endregion
    }

    [Fact]
    public async Task HttpResponseNested()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://httpbin.org/get");

        await Verify(new
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

        var result = await client.GetAsync("https://httpbin.org/image/png");

        await Verify(result);
    }

    #region HttpResponse
    [Fact]
    public async Task HttpResponse()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://httpbin.org/get");

        await Verify(result);
    }
    #endregion

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

        await Verify(recording.Sends)
            .ModifySerialization(x => x.IgnoreMembers("Date"));

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
        await client.GetAsync("https://httpbin.org/status/undefined");

        await Verify(recording.Sends)
            .ModifySerialization(x => x.IgnoreMembers("Date"));

        #endregion
    }
}