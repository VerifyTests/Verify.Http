using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class MyControllerTests
{
    #region MyControllerTest
    [Test]
    public Task Test()
    {
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        var controller = new MyController
        {
            ControllerContext = context
        };

        var result = controller.Method("inputValue");
        return Verifier.Verify(
            new
            {
                result,
                context
            });
    }
    #endregion
}