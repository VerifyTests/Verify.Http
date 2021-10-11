using System.Net.Http;
using VerifyTests;

static class HttpResponseSplitterResult
{
    public static ConversionResult Convert(HttpResponseMessage instance)
    {
        var targets = new List<Target>();
        var content = instance.Content;
        if (content.TryGetExtension(out var extension))
        {
            if (EmptyFiles.Extensions.IsText(extension))
            {
                return new(instance, targets);
            }

            targets.Add(new(extension, content.ReadAsStream()));
        }

        return new(
            new
            {
                instance.Version,
                instance.StatusCode,
                instance.IsSuccessStatusCode,
                instance.ReasonPhrase,
                Cookies = instance.Headers.Cookies(),
                Headers = instance.Headers.NotCookies(),
#if NET5_0_OR_GREATER || NETSTANDARD2_1
                instance.TrailingHeaders,
#endif
                instance.RequestMessage
            },
            targets);
    }
}