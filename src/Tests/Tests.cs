using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VerifyNUnit;
using NUnit.Framework;
using VerifyTests;

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
        var collection = new ServiceCollection();
        // Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
        var (builder, sends) = collection.AddRecordingHttpClient();

        await using var provider = collection.BuildServiceProvider();

        // Resolve a HttpClient
        // All http calls done at any resolved client will be added to `sends`
        var client = provider.GetRequiredService<HttpClient>();

        // Some code that does some http calls
        await client.GetAsync("https://httpbin.org/");

        await Verifier.Verify(sends)
            // Ignore some headers that change per request
            .ModifySerialization(x => x.IgnoreMembers("Date"));
    }
}