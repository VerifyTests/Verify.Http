public static class OtherModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
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
            "X-Timer",
            "ETag");
}