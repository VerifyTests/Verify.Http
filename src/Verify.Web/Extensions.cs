using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

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

    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static async Task<string> ReadAsString(this Stream stream)
    {
        stream.MoveToStart();
        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers)
    {
        return headers
            .ToDictionary(x => x.Key, x => string.Join('|', x.Value));
    }

    public static Dictionary<string, string?> NotCookies(this HttpHeaders headers)
    {
        return headers
            .Where(x => x.Key != HeaderNames.SetCookie)
            .ToDictionary(x => x.Key, x => x.Value.ToString());
    }

    public static Dictionary<string, string> NotCookies(this IHeaderDictionary headers)
    {
        return headers
            .Where(x => x.Key != HeaderNames.SetCookie)
            .ToDictionary(x => x.Key, x => x.Value.ToString());
    }

    public static Dictionary<string, string> Cookies(this HttpHeaders headers)
    {
        return headers
            .Where(x => x.Key == HeaderNames.SetCookie)
            .Select(x =>
            {
                var stringSegment = x.Value.Single();
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(x => x.Name.Value, x => x.Value.Value);
    }

    public static Dictionary<string, string> Cookies(this IHeaderDictionary headers)
    {
        return headers
            .Where(x => x.Key == HeaderNames.SetCookie)
            .Select(x =>
            {
                var stringSegment = x.Value.Single();
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(x => x.Name.Value, x => x.Value.Value);
    }
}