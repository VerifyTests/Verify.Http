using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VerifyTests.Web;
using Microsoft.Extensions.Options;

namespace VerifyTests
{
    public static class VerifyWeb
    {
        public static (IHttpClientBuilder builder, IEnumerable<LoggedSend> sends) AddRecordingHttpClient(
            this ServiceCollection collection,
            string? name = null)
        {
            name ??= Options.DefaultName;

            var builder = collection.AddHttpClient(name);
            var handler = new RecordingHandler();
            builder.AddHttpMessageHandler(() => handler);
            return (builder, handler.Sends);
        }

        public static void Enable()
        {
            VerifierSettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializer =>
                {
                    var converters = serializer.Converters;
                    converters.Add(new HttpResponseConverter());
                    converters.Add(new ChallengeResultConverter());
                    converters.Add(new ActionResultConverter());
                    converters.Add(new ContentResultConverter());
                    converters.Add(new ControllerContextConverter());
                    converters.Add(new EmptyResultConverter());
                    converters.Add(new FileContentResultConverter());
                    converters.Add(new FileStreamResultConverter());
                    converters.Add(new PhysicalFileResultConverter());
                    converters.Add(new VirtualFileResultConverter());
                    converters.Add(new ForbidResultConverter());
                    converters.Add(new JsonResultConverter());
                    converters.Add(new LocalRedirectResultConverter());
                    converters.Add(new ObjectResultConverter());
                    converters.Add(new AcceptedAtActionResultConverter());
                    converters.Add(new AcceptedAtRouteResultConverter());
                    converters.Add(new AcceptedResultConverter());
                    converters.Add(new BadRequestObjectResultConverter());
                    converters.Add(new ConflictObjectResultConverter());
                    converters.Add(new CreatedAtActionResultConverter());
                    converters.Add(new CreatedAtRouteResultConverter());
                    converters.Add(new CreatedResultConverter());
                    converters.Add(new NotFoundObjectResultConverter());
                    converters.Add(new OkObjectResultConverter());
                    converters.Add(new UnauthorizedObjectResultConverter());
                    converters.Add(new UnprocessableEntityObjectResultConverter());
                    converters.Add(new PartialViewResultConverter());
                    converters.Add(new RedirectResultConverter());
                    converters.Add(new RedirectToActionResultConverter());
                    converters.Add(new RedirectToPageResultConverter());
                    converters.Add(new RedirectToRouteResultConverter());
                    converters.Add(new SignInResultConverter());
                    converters.Add(new SignOutResultConverter());
                    converters.Add(new StatusCodeResultConverter());
                    converters.Add(new BadRequestResultConverter());
                    converters.Add(new ConflictResultConverter());
                    converters.Add(new NoContentResultConverter());
                    converters.Add(new NotFoundResultConverter());
                    converters.Add(new OkResultConverter());
                    converters.Add(new UnauthorizedResultConverter());
                    converters.Add(new UnprocessableEntityResultConverter());
                    converters.Add(new UnsupportedMediaTypeResultConverter());
                    converters.Add(new ConflictResultConverter());
                    converters.Add(new ViewComponentResultConverter());
                    converters.Add(new ViewResultConverter());
                    converters.Add(new PageResultConverter());
                });
            });
        }
    }
}