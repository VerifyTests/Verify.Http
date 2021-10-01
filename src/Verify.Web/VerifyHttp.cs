using Microsoft.Extensions.DependencyInjection;
using VerifyTests.Web;
using Microsoft.Extensions.Options;

namespace VerifyTests
{
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
            VerifierSettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializer =>
                {
                    var converters = serializer.Converters;
                    converters.Add(new HttpResponseMessageConverter());
                    converters.Add(new HttpRequestMessageConverter());
                });
            });
        }
    }
}