#if NET6_0_OR_GREATER
using System.Net.Http.Json;
#endif

static class HttpExtensions
{
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

#if NET48
    public static Task<string> ReadAsStringAsync(this HttpContent request, Cancel cancel) =>
        request.ReadAsStringAsync();
#endif

    public static bool IsDefaultVersion(this HttpRequestMessage request) =>
        request.Version == defaultRequestVersion;

#if NET6_0_OR_GREATER
    public static bool IsDefaultVersionPolicy(this HttpRequest request) =>
        request.VersionPolicy == defaultRequestVersionPolicy;

    public static bool IsDefaultVersionPolicy(this HttpRequestMessage request) =>
        request.VersionPolicy == defaultRequestVersionPolicy;
#endif

    static HttpExtensions()
    {
        var request = new HttpRequestMessage();
        defaultRequestVersion = request.Version;
#if NET6_0_OR_GREATER
        defaultRequestVersionPolicy = request.VersionPolicy;
#endif
    }

    static Version defaultRequestVersion;
#if NET6_0_OR_GREATER
    static HttpVersionPolicy defaultRequestVersionPolicy;
#endif

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
#if NET6_0_OR_GREATER
        if (content is JsonContent)
        {
            subType = "json";
            return true;
        }
#endif

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

        if (mediaType == "application/json" || mediaType.EndsWith("+json"))
        {
            subType = "json";
            return true;
        }

        return false;
    }
}
