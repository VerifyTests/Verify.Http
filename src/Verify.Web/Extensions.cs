using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

static class Extensions
{
    public static bool IsText(this HttpContent content)
    {
        var contentType = content.Headers.ContentType;
        if (contentType?.MediaType == null)
        {
            return false;
        }

        return contentType.MediaType.StartsWith("text");
    }

    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
    {
        return headers
            .ToDictionary(x => x.Key, x => string.Join('|', x.Value));
    }
}