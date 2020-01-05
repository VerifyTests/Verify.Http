namespace Verify.Web
{
    public static class VerifyWeb
    {
        public static void Enable()
        {
            SharedVerifySettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializer =>
                {
                    var converters = serializer.Converters;
                    converters.Add(new ControllerContextConverter());
                    converters.Add(new ChallengeResultConverter());
                    converters.Add(new ActionResultConverter());
                });
            });
        }
    }
}