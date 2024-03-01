namespace VerifyTests;

public static class VerifyHttp
{
    public static (IHttpClientBuilder builder, RecordingHandler recording) AddRecordingHttpClient(
        this ServiceCollection collection,
        string? name = null)
    {
        name ??= Options.DefaultName;

        var builder = collection.AddHttpClient(name);
        var recording = AddRecording(builder);
        return (builder, recording);
    }

    public static RecordingHandler AddRecording(this IHttpClientBuilder builder)
    {
        var recording = new RecordingHandler();
        builder.AddHttpMessageHandler(() => recording);
        return recording;
    }

    public static bool Initialized { get; private set; }

    static HttpListener listener = new();

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
        VerifierSettings.RegisterFileConverter<HttpResponseMessage>(
            (instance, _) => HttpResponseSplitterResult.Convert(instance));
        VerifierSettings
            .AddExtraSettings(serializer =>
            {
                var converters = serializer.Converters;
                converters.Insert(0, new HttpStatusCodeConverter());
                converters.Add(new XmlNodeConverter());
                converters.Add(new HttpMethodConverter());
                converters.Add(new UriConverter());
                converters.Add(new HttpHeadersConverter());
                converters.Add(new HttpContentHeadersConverter());
                converters.Add(new HttpContentConverter());
                converters.Add(new HttpRequestMessageConverter());
                converters.Add(new HttpResponseMessageConverter());
                converters.Add(new HttpRequestConverter());
                converters.Add(new HttpResponseConverter());
                converters.Add(new MockHttpClientConverter());
                converters.Add(new MockHttpHandlerConverter());
            });
    }
}