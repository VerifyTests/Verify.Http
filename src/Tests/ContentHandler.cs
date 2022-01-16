using System.Net;

class ContentHandler : DelegatingHandler
{
    StringContent content;

    public ContentHandler(StringContent content)
    {
        this.content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
    {
        var result = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content,
        };

        return Task.FromResult(result);
    }
}