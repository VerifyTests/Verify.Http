class ContentHandler : DelegatingHandler
{
    StringContent content;

    public ContentHandler(StringContent content) =>
        this.content = content;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        var result = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content,
        };

        return Task.FromResult(result);
    }
}