using Microsoft.Extensions.DependencyInjection;
using VerifyTests.Http;
using Microsoft.Extensions.Options;

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

    public static void Enable()
    {
        VerifierSettings.RegisterFileConverter<HttpResponseMessage>(
            (instance, _) => HttpResponseSplitterResult.Convert(instance));
        VerifierSettings
            .AddExtraSettings(serializer =>
            {
                var converters = serializer.Converters;
                converters.Add(new HttpMethodConverter());
                converters.Add(new UriConverter());
                converters.Add(new HttpHeadersConverter());
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