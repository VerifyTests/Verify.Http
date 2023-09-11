#if NET6_0_OR_GREATER

namespace VerifyTests.Http;

public static class HttpRecording
{
    static HttpListener listener = new();

    public static void Enable()
    {
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);

        VerifierSettings.RegisterJsonAppender(_ =>
        {
            if (!TryFinishRecording(out var entries))
            {
                return null;
            }

            if (entries.Count == 0)
            {
                return null;
            }

            return new("httpCalls", entries);
        });
    }

    public static void StartRecording() =>
        listener.Start();

    public static IReadOnlyList<HttpCall> FinishRecording() =>
        listener.Finish();

    public static bool TryFinishRecording([NotNullWhen(true)] out IReadOnlyList<HttpCall>? entries) =>
        listener.TryFinish(out entries);
}

#endif