using Verify.Web;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        #region Enable
        VerifyWeb.Enable();
        #endregion
    }
}