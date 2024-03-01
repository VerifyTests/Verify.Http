static class HttpResponseSplitterResult
{
    public static ConversionResult Convert(HttpResponseMessage instance)
    {
        var targets = new List<Target>();
        var content = instance.Content;
        if (content.TryGetExtension(out var extension))
        {
            if (FileExtensions.IsTextExtension(extension))
            {
                return new(instance, targets);
            }

            targets.Add(new(extension, content.ReadAsStream()));
        }

        return new(
            new
            {
                instance.Version,
                Status = instance.StatusText(),
                Cookies = instance.Headers.Cookies(),
                Headers = instance.Headers.NotCookies(),
                TrailingHeaders = instance.TrailingHeaders.Simplify(),
                instance.RequestMessage
            },
            targets);
    }
}