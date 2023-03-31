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
    public static Task<string> ReadAsStringAsync(this HttpContent request, Cancellation cancellation) =>
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

    static Dictionary<string, string> mappings = new(StringComparer.OrdinalIgnoreCase)
    {
        //extra
        {"application/graphql", "gql"},
        {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"},
        {"application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx"},
        {"application/vnd.ms-word.document.macroEnabled.12", "docm"},
        {"application/vnd.ms-word.template.macroEnabled.12", "dotm"},
        {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"},
        {"application/vnd.openxmlformats-officedocument.spreadsheetml.template", "xltx"},
        {"application/vnd.ms-excel.sheet.macroEnabled.12", "xlsm"},
        {"application/vnd.ms-excel.template.macroEnabled.12", "xltm"},
        {"application/vnd.ms-excel.addin.macroEnabled.12", "xlam"},
        {"application/vnd.ms-excel.sheet.binary.macroEnabled.12", "xlsb"},
        {"application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx"},
        {"application/vnd.openxmlformats-officedocument.presentationml.template", "potx"},
        {"application/vnd.openxmlformats-officedocument.presentationml.slideshow", "ppsx"},
        {"application/vnd.ms-powerpoint.addin.macroEnabled.12", "ppam"},
        {"application/vnd.ms-powerpoint.presentation.macroEnabled.12", "pptm"},
        {"application/vnd.ms-powerpoint.template.macroEnabled.12", "potm"},
        {"application/vnd.ms-powerpoint.slideshow.macroEnabled.12", "ppsm"},

        {"application/fsharp-script", "fsx"},
        {"application/msaccess", "adp"},
        {"application/msword", "doc"},
        {"application/octet-stream", "bin"},
        {"application/onenote", "one"},
        {"application/postscript", "eps"},
        {"application/soap+xml", "xml"},
        {"application/step", "step"},
        {"application/vnd.ms-excel", "xls"},
        {"application/vnd.ms-powerpoint", "ppt"},
        {"application/vnd.ms-works", "wks"},
        {"application/vnd.visio", "vsd"},
        {"application/x-director", "dir"},
        {"application/x-msdos-program", "exe"},
        {"application/x-shockwave-flash", "swf"},
        {"application/x-x509-ca-cert", "cer"},
        {"application/x-zip-compressed", "zip"},
        {"application/xhtml+xml", "xhtml"},
        {"application/xrd+xml", "xml"},
        {"application/xml", "xml"},
        {"audio/aac", "aac"},
        {"audio/aiff", "aiff"},
        {"audio/basic", "snd"},
        {"audio/mid", "midi"},
        {"audio/mp4", "m4a"},
        {"audio/wav", "wav"},
        {"audio/x-m4a", "m4a"},
        {"audio/x-mpegurl", "m3u"},
        {"audio/x-pn-realaudio", "ra"},
        {"audio/x-smd", "smd"},
        {"image/bmp", "bmp"},
        {"image/heic", ".heic"},
        {"image/heic-sequence", "heics"},
        {"image/jpeg", "jpg"},
        {"image/gif", "gif"},
        {"image/pict", "pic"},
        {"image/png", "png"},
        {"image/x-png", "png"},
        {"image/svg+xml", "svg"},
        {"image/tiff", "tiff"},
        {"image/x-macpaint", "mac"},
        {"image/x-quicktime", "qti"},
        {"message/rfc822", "eml"},
        {"text/calendar", "ics"},
        {"text/html", "html"},
        {"text/plain", "txt"},
        {"text/scriptlet", "wsc"},
        {"text/xml", "xml"},
        {"text/csv", "csv"},
        {"video/3gpp", "3gp"},
        {"video/3gpp2", "3gp2"},
        {"video/mp4", "mp4"},
        {"video/mpeg", "mpg"},
        {"video/quicktime", "mov"},
        {"video/vnd.dlna.mpeg-tts", "m2t"},
        {"video/x-dv", "dv"},
        {"video/x-la-asf", "lsf"},
        {"video/x-ms-asf", "asf"},
        {"x-world/x-vrml", "xof"}
    };

    public static bool TryGetExtension(this HttpContent? content, [NotNullWhen(true)] out string? extension)
    {
        var contentType = content?.Headers.ContentType;
        if (contentType is null)
        {
            extension = null;
            return false;
        }

        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            extension = null;
            return false;
        }

        if (mappings.TryGetValue(mediaType, out extension))
        {
            return true;
        }

        if (IsJsonMediaType(mediaType, out extension))
        {
            extension = "json";
            return true;
        }

        return false;
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

        if (IsJson(contentType))
        {
            subType = "json";
            return true;
        }

        if (IsText(contentType, out subType))
        {
            return true;
        }

        return content is StringContent;
    }

    static bool IsText(MediaTypeHeaderValue contentType, [NotNullWhen(true)] out string? subType)
    {
        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            subType = null;
            return false;
        }

        var split = mediaType.Split('/');
        subType = split[1];
        if (mappings.TryGetValue(mediaType, out var extension))
        {
            return FileExtensions.IsText(extension);
        }

        return false;
    }

    static bool IsJson(MediaTypeHeaderValue contentType)
    {
        var mediaType = contentType.MediaType;
        if (mediaType is null)
        {
            return false;
        }

        return IsJsonMediaType(mediaType, out _);
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
