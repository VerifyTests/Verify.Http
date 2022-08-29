public static class ModuleInitializer
{
    #region Enable

    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();

        #endregion

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

#if NET48
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ModuleInitializerAttribute : Attribute
    {
    }
}
#endif