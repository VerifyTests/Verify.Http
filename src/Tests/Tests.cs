using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
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
    [Fact]
    public Task ChallengeResult()
    {
        var result = new ChallengeResult(
            "scheme",
            new AuthenticationProperties(
                new Dictionary<string, string>
                {
                    {"key", "value"}
                }));
        return VerifyResult(result);
    }

    private Task VerifyResult(ActionResult result)
    {
        return Verify(
            new
            {
                result
            });
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}