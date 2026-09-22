public static class OtherModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // TestServer listens on a random port
        VerifierSettings.AddScrubber(_ => _.Replace(TestServer.Root, "http://test-server"));
        VerifierSettings.IgnoreMembers(
            "X-Fastly-Request-ID",
            "X-GitHub-Request-Id",
            "X-Served-By",
            "X-Timer",
            "CF-RAY",
            "Server-Timing",
            "Report-To",
            "traceparent",
            "origin",
            "Source-Age",
            "X-Cache-Hits",
            "X-Cache",
            "X-Timer",
            "ETag");
    }
}