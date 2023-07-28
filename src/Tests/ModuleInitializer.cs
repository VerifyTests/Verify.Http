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
            "X-GitHub-Request-Id",
            "origin",
            "Date",
            "Server",
            "X-Fastly-Request",
            "Source-Age",
            "X-Fastly-Request-ID",
            "X-Served-By",
            "X-Cache-Hits",
            "X-Served-By",
            "X-Cache",
            "Content-Length",
            "X-Timer");
        VerifierSettings
            .ScrubLinesContaining(
                "Traceparent",
                "Date",
                "X-Amzn-Trace-Id",
                "Content-Length");
    }
}