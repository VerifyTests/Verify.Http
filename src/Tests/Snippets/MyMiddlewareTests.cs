using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class MyMiddlewareTests :
    VerifyBase
{
    #region MyMiddlewareTest
    [Fact]
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

        await Verify(
            new
            {
                context.Response,
                nextCalled
            });
    }
    #endregion

    public MyMiddlewareTests(ITestOutputHelper output) :
        base(output)
    {
    }
}