public static class ModuleInit
{
    #region Enable

    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Initialize();
#if NET7_0
        HttpRecording.Enable();
#endif
    }

    #endregion

    [ModuleInitializer]
    public static void InitializeOther()
    {
        VerifierSettings.InitializePlugins();

        VerifierSettings.IgnoreMembers(
            "Content-Length",
            "traceparent",
            "Traceparent",
            "X-Amzn-Trace-Id",
            "origin");
        VerifierSettings
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "Content-Length");
    }
}