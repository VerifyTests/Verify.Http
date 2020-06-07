using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class MyMiddlewareTests
{
    #region MyMiddlewareTest
    [Test]
    public async Task Test()
    {
        var nextCalled = false;
        var middleware = new MyMiddleware(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

        var context = new DefaultHttpContext();
        await middleware.Invoke(context);

        await Verifier.Verify(
            new
            {
                context.Response,
                nextCalled
            });
    }
    #endregion
}