using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VerifyNUnit;
using NUnit.Framework;
using VerifyTests;
using VerifyTests.Web;

[TestFixture]
public class Tests
{
    /**

ContentResult
EmptyResult
FileContentResult
FileStreamResult
PhysicalFileResult
VirtualFileResult
ForbidResult
JsonResult
LocalRedirectResult
ObjectResult
AcceptedAtActionResult
AcceptedAtRouteResult
AcceptedResult
BadRequestObjectResult
ConflictObjectResult
CreatedAtActionResult
CreatedAtRouteResult
CreatedResult
NotFoundObjectResult
OkObjectResult
UnauthorizedObjectResult
UnprocessableEntityObjectResult
BadRequestErrorMessageResult
ExceptionResult
InvalidModelStateResult
NegotiatedContentResult<T>
ResponseMessageResult
PartialViewResult
RedirectResult
RedirectToActionResult
RedirectToPageResult
RedirectToRouteResult
SignInResult
SignOutResult
StatusCodeResult
BadRequestResult
ConflictResult
NoContentResult
NotFoundResult
OkResult
UnauthorizedResult
UnprocessableEntityResult
UnsupportedMediaTypeResult
ConflictResult
InternalServerErrorResult
ViewComponentResult
ViewResult
PageResult
    **/
    [Test]
    public Task ChallengeResult()
    {
        var result = new ChallengeResult(
            "scheme",
            new AuthenticationProperties(
                new Dictionary<string, string?>
                {
                    {"key", "value"}
                }));
        return Verifier.Verify(result);
    }


    [Test]
    public async Task HttpClientRecording()
    {
        #region HttpClientRecording
        var collection = new ServiceCollection();
        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var (builder, recording) = collection.AddRecordingHttpClient();

        await using var provider = collection.BuildServiceProvider();

        // Resolve a HttpClient
        // All http calls done at any resolved client will be added to `recording.Sends`
        var client = provider.GetRequiredService<HttpClient>();

        // Some code that does some http calls
        await client.GetAsync("https://httpbin.org/status/undefined");

        await Verifier.Verify(recording.Sends)
            // Ignore some headers that change per request
            .ModifySerialization(x => x.IgnoreMembers("Date"));
        #endregion
    }

    [Test]
    public async Task PauseResume()
    {
        #region HttpClientPauseResume
        var collection = new ServiceCollection();
        var (builder, recording) = collection.AddRecordingHttpClient();

        await using var provider = collection.BuildServiceProvider();

        var client = provider.GetRequiredService<HttpClient>();

        // Recording is enabled by default. So Pause to stop recording
        recording.Pause();
        await client.GetAsync("https://www.google.com/");

        // Resume recording
        recording.Resume();
        await client.GetAsync("https://httpbin.org/status/undefined");

        await Verifier.Verify(recording.Sends)
            .ModifySerialization(x => x.IgnoreMembers("Date"));
        #endregion
    }

    [Test]
    public async Task RecordingFullControl()
    {
        #region HttpClientRecordingExplicit
        var collection = new ServiceCollection();

        var builder = collection.AddHttpClient("name");

        // Change to not recording at startup
        var recording = new RecordingHandler(recording: false);

        builder.AddHttpMessageHandler(() => recording);

        await using var provider = collection.BuildServiceProvider();

        var factory = provider.GetRequiredService<IHttpClientFactory>();

        var client = factory.CreateClient("name");

        await client.GetAsync("https://www.google.com/");

        recording.Resume();
        await client.GetAsync("https://httpbin.org/status/undefined");

        await Verifier.Verify(recording.Sends)
            .ModifySerialization(x => x.IgnoreMembers("Date"));
        #endregion
    }
}