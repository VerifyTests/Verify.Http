#if NET6_0_OR_GREATER

namespace VerifyTests.Http;

public static class HttpRecording
{
    static HttpListener listener = new();

    public static void Enable()
    {
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);

        VerifierSettings.RegisterJsonAppender(_ =>
        {
            if (!TryFinishRecording(out var entries))
            {
                return null;
            }

            return new("httpCalls", entries!);
        });
    }

    public static void StartRecording() =>
        listener.Start();

    public static IEnumerable<HttpCall> FinishRecording() =>
        listener.Finish();

    public static bool TryFinishRecording(out IEnumerable<HttpCall>? entries) =>
        listener.TryFinish(out entries);
}

#endif