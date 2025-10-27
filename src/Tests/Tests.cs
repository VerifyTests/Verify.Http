// ReSharper disable UnusedVariable
// ReSharper disable UnusedParameter.Global

[TestFixture]
public class Tests
{
    #region IgnoreHeader

    [Test]
    public async Task IgnoreHeader()
    {
        using var client = new HttpClient();

        using var result = await client.GetAsync("https://httpcan.org/get");

        await Verify(result)
            .IgnoreMembers(
                "Server",
                "Access-Control-Allow-Credentials");
    }

    #endregion

    #region ScrubHttpTextResponse

    [Test]
    public async Task ScrubHttpTextResponse()
    {
        using var client = new HttpClient();

        using var result = await client.GetAsync("https://httpcan.org/html");

        await Verify(result)
            .ScrubHttpTextResponse(_ => _.Replace("Herman Melville - Moby-Dick", "New title"));
    }

    #endregion

    [Test]
    public async Task IgnoreAuth()
    {
        using var client = new HttpClient
        {
            DefaultRequestHeaders =
            {
                Authorization = new("Basic", "key")
            }
        };
        Recording.Start();
        var result = await client.GetAsync("https://httpcan.org/get");
        await Verify();
    }

    [Test]
    public async Task JsonGet()
    {
        Recording.Start();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://httpcan.org/json");

        await VerifyJson(result);
    }

    [Test]
    public async Task TestHttpRecordingWithResponse()
    {
        Recording.Start();

        using var client = new HttpClient();

        var result = await client.GetStringAsync("https://httpcan.org/json");

        await Verify(result);
    }

    #region ServiceThatDoesHttp

    // Resolve a HttpClient. All http calls done at any
    // resolved client will be added to `recording.Sends`
    public class MyService(HttpClient client)
    {
        public Task MethodThatDoesHttp() =>
            // Some code that does some http calls
            client.GetAsync("https://httpcan.org/get");
    }

    #endregion

    [Test]
    public async Task MediaTypePlainTextIsRecorded()
    {
        const string content = "string content 123";
        var handler = new RecordingHandler
        {
            InnerHandler = new ContentHandler(new(content, Encoding.UTF8))
        };
        using var client = new HttpClient(handler);

        using var response = await client.GetAsync("https://dont-care.org/get");

        AreEqual(content, handler.Sends.Single().ResponseContent);
    }

    [TestCase("application/json")]
    [TestCase("application/foo+json")]
    public async Task MediaTypeApplicationJsonIsRecorded(string mediaType)
    {
        const string content = "{ \"age\": 1234 }";
        var handler = new RecordingHandler
        {
            InnerHandler = new ContentHandler(new(content, Encoding.UTF8, mediaType))
        };
        using var client = new HttpClient(handler);

        using var response = await client.GetAsync("https://dont-care.org/get");

        AreEqual(content, handler.Sends.Single().ResponseContent);
    }
#if DEBUG

    #region HttpRecording

    [Test]
    public async Task TestHttpRecording()
    {
        Recording.Start();

        var sizeOfResponse = await MethodThatDoesHttpCalls();

        await Verify(
                new
                {
                    sizeOfResponse
                })
            .IgnoreMembers("Expires", "Date")
            .ScrubLinesContaining("\"version\"");
    }

    static async Task<int> MethodThatDoesHttpCalls()
    {
        using var client = new HttpClient();

        var jsonResult = await client.GetStringAsync("https://httpcan.org/json");
        var ymlResult = await client.GetStringAsync("https://httpcan.org/xml");
        return jsonResult.Length + ymlResult.Length;
    }

    #endregion

    #region HttpRecordingExplicit

    [Test]
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
            IsTrue(call.Duration < threshold);
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

    [Test]
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
            .IgnoreMember("Date");

        #endregion
    }

    [Test]
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
            .IgnoreMember("Date");

        #endregion
    }

    [Test]
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

    [Test]
    public async Task ImageHttpResponse()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/src/icon.png");

        await Verify(result);
    }

    #region HttpResponse

    [Test]
    public async Task HttpResponse()
    {
        using var client = new HttpClient();

        var result = await client.GetAsync("https://httpcan.org/json");

        await Verify(result);
    }

    #endregion

    [Test]
    public Task HttpStatusCodeTest() =>
        Verify(HttpStatusCode.Ambiguous)
            .UniqueForRuntime();

    [Test]
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
            .ScrubInlineDateTimes("R");

        #endregion
    }

    [Test]
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

        await client.GetAsync("https://httpcan.org/html");

        recording.Resume();
        await client.GetAsync("https://httpcan.org/json");

        await Verify(recording.Sends)
            .ScrubInlineDateTimes("R");

        #endregion
    }

    [Test]
    public async Task WithOwnListener()
    {
        using var _ = DiagnosticListener.AllListeners.Subscribe(new MyListener());
        using var client = new HttpClient();
        using var response = await client.GetAsync("https://httpcan.org/json");
        await Verify(response.StatusCode);
    }

    public class MyListener : IObserver<DiagnosticListener>,
        IDisposable
    {
        private readonly ConcurrentQueue<IDisposable> subscriptions = [];

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name != "HttpHandlerDiagnosticListener")
            {
                return;
            }

            subscriptions.Enqueue(value.SubscribeWithAdapter(this));
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut")]
        public void IsEnabled()
        {
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public virtual void OnHttpRequestOutStart(HttpRequestMessage request)
        {
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public virtual void OnHttpRequestOutStop(HttpRequestMessage request, HttpResponseMessage response, TaskStatus status)
        {
        }
    }
}