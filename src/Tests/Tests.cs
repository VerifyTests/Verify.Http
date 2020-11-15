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
        return Verifier.Verify((ActionResult) result);
    }

    [Test]
    public async Task HttpClientRecording()
    {
        var collection = new ServiceCollection();
        var (builder, handler) = collection.AddRecordingHttpClient();

        var serviceProvider = collection.BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        var client = httpClientFactory.CreateClient();

        await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify.Web/master/license.txt");

        await Verifier.Verify(handler.Sends)
            .ModifySerialization(x=>x.IgnoreMember("X-Fastly-Request-ID"));
    }
}