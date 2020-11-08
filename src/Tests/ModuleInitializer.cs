using System.Runtime.CompilerServices;
using VerifyTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        #region Enable
        VerifyWeb.Enable();
        #endregion
    }
}