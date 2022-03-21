using System.Collections.Specialized;
using System.Web;

class UriConverter :
    WriteOnlyJsonConverter<Uri>
{
    public override void Write(VerifyJsonWriter writer, Uri value)
    {
        if (ShouldUseOriginalString(value))
        {
            writer.WriteValue(value.OriginalString);
            return;
        }

        var path = GetPath(value);

        writer.Serialize(
            new UriWrapper
            {
                Path = path,
                Query = HttpUtility.ParseQueryString(value.Query)
            });
    }

    public static bool ShouldUseOriginalString(Uri value) =>
        value.IsAbsoluteUri == false ||
        string.IsNullOrWhiteSpace(value.Query);

    static string GetPath(Uri value)
    {
        var scheme = value.Scheme;
        var port = value.Port;
        if (scheme == "http" && port == 80)
        {
            return $"http://{value.Host}{value.AbsolutePath}";
        }

        if (scheme == "https" && port == 443)
        {
            return $"https://{value.Host}{value.AbsolutePath}";
        }

        return $"{scheme}://{value.Host}:{port}{value.AbsolutePath}";
    }

    class UriWrapper
    {
        public string Path { get; set; } = null!;
        public NameValueCollection Query { get; set; }= null!;
    }
}