class ContentHandler(StringContent content) :
    DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        var result = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content,
        };

        return Task.FromResult(result);
    }
}