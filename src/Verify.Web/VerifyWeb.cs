namespace Verify.Web
{
    public static class VerifyWeb
    {
        public static void Enable()
        {
            SharedVerifySettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializerSettings =>
                {
                    var converters = serializerSettings.Converters;
                    converters.Add(new ControllerContextConverter());
                    converters.Add(new ActionResultConverter());
                });
            });
        }
    }
}