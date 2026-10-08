using System.Net.Http.Json;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

static class HttpExtensions
{
    internal static string ReadAsString(this HttpContent content)
    {
        if (content is FileContent fileContent)
        {
            return fileContent.ReadFileAsString();
        }

        return content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers) =>
        headers
            .ToDictionary(_ => _.Key, _ => string.Join('|', _.Value));

    public static Dictionary<string, object> Simplify(this HttpHeaders headers) =>
        headers
            .OrderBy(_ => _.Key.ToLowerInvariant())
            .ToDictionary(
                _ => _.Key,
                object (_) =>
                {
                    var values = _.Value.ToList();
                    var key = _.Key.ToLowerInvariant();
                    if (key is "date" or "expires" or "last-modified")
                    {
                        if (DateTime.TryParse(values.First(), out var date))
                        {
                            return date;
                        }
                    }

                    if (key is "authorization")
                    {
                        return "{Scrubbed}";
                    }

                    return string.Join(',', values);
                });

    public static Dictionary<string, object> NotCookies(this HttpHeaders headers) =>
        headers
            .Simplify()
            .Where(_ => _.Key != "Set-Cookie")
            .ToDictionary(_ => _.Key, _ => _.Value);

    public static Dictionary<string, string?> Cookies(this HttpHeaders headers) =>
        headers
            .Simplify()
            .Where(_ => _.Key == "Set-Cookie")
            .Select(_ =>
            {
                var stringSegment = (string)_.Value;
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(_ => _.Name.Value!, _ => _.Value.Value);

    public static string StatusText(this HttpResponseMessage instance)
    {
        var status = instance.StatusCode;
        if (instance.ReasonPhrase == null)
        {
            return $"{(int)status} {status}";
        }

        return $"{(int)status} {instance.ReasonPhrase}";
    }

    public static bool IsDefaultVersion(this HttpRequest request) =>
        request.Version == defaultRequestVersion;

    public static bool IsDefaultVersion(this HttpRequestMessage request) =>
        request.Version == defaultRequestVersion;

    public static bool IsDefaultVersion(this HttpResponseMessage request) =>
        request.Version == defaultRequestVersion;

    public static bool IsDefaultVersionPolicy(this HttpRequest request) =>
        request.VersionPolicy == defaultRequestVersionPolicy;

    public static bool IsDefaultVersionPolicy(this HttpRequestMessage request) =>
        request.VersionPolicy == defaultRequestVersionPolicy;

    static HttpExtensions()
    {
        var request = new HttpRequestMessage();
        defaultRequestVersion = request.Version;
        defaultRequestVersionPolicy = request.VersionPolicy;
    }

    static Version defaultRequestVersion;
    static HttpVersionPolicy defaultRequestVersionPolicy;

    public static (string? content, object? prettyContent) TryReadStringContent(this HttpContent? content)
    {
        if (content == null)
        {
            return (null, null);
        }

        if (!content.IsText(out var subType))
        {
            return (null, null);
        }

        var stringContent = content.ReadAsString();
        object prettyContent = stringContent;
        if (subType == "json")
        {
            try
            {
                prettyContent = JToken.Parse(stringContent);
            }
            catch
            {
            }
        }
        else if (subType == "xml")
        {
            try
            {
                prettyContent = XDocument.Parse(stringContent);
            }
            catch
            {
            }
        }
        else if (subType == "formUrlEncoded")
        {
            try
            {
                prettyContent = stringContent
                    .Split('&')
                    .Select(_ => _.Split('='))
                    .ToDictionary(_ => _[0], _ => _[1]);
            }
            catch
            {
            }
        }

        return (stringContent, prettyContent);
    }

    public static bool TryGetExtension(this HttpContent? content, [NotNullWhen(true)] out string? extension)
    {
        var contentType = content?.Headers.ContentType;
        if (contentType is null)
        {
            extension = null;
            return false;
        }

        return TryGetExtension(contentType, out extension);
    }

    static bool TryGetExtension(this MediaTypeHeaderValue contentType, [NotNullWhen(true)] out string? extension)
    {
        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            extension = null;
            return false;
        }

        return ContentTypes.TryGetExtension(mediaType, out extension);
    }

    public static bool IsText(this HttpContent content) =>
        content.IsText(out _);

    public static bool IsText(this HttpContent content, out string? subType)
    {
        if (content is JsonContent)
        {
            subType = "json";
            return true;
        }

        if (content is FormUrlEncodedContent)
        {
            subType = "formUrlEncoded";
            return true;
        }

        var contentType = content.Headers.ContentType;
        if (contentType is null)
        {
            subType = null;
            return content is StringContent;
        }

        if (IsJson(contentType, out subType))
        {
            return true;
        }

        if (IsText(contentType, out subType))
        {
            return true;
        }

        return content is StringContent;
    }

    static bool IsText(MediaTypeHeaderValue contentType, [NotNullWhen(true)] out string? extension)
    {
        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            extension = null;
            return false;
        }

        return ContentTypes.IsText(mediaType, out extension);
    }

    static bool IsJson(MediaTypeHeaderValue contentType, [NotNullWhen(true)] out string? subType)
    {
        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            subType = null;
            return false;
        }

        return IsJsonMediaType(mediaType, out subType);
    }

    static bool IsJsonMediaType(string mediaType, [NotNullWhen(true)] out string? subType)
    {
        subType = null;

        if (mediaType == "application/json" ||
            mediaType.EndsWith("+json"))
        {
            subType = "json";
            return true;
        }

        return false;
    }
}
