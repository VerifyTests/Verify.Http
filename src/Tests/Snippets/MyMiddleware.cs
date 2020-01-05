using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class MyMiddleware
{
    RequestDelegate next;

    public MyMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("headerKey", "headerValue");
        await next(context);
    }
}