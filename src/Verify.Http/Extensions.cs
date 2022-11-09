using Microsoft.Net.Http.Headers;

static class Extensions
{
#if(!NET5_0_OR_GREATER)
    internal static Stream ReadAsStream(this HttpContent content) =>
        content.ReadAsStreamAsync().GetAwaiter().GetResult();
#endif

    internal static string ReadAsString(this HttpContent content) =>
        content.ReadAsStringAsync().GetAwaiter().GetResult();

    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers) =>
        headers
            .ToDictionary(x => x.Key, x => string.Join("|", x.Value));

    public static Dictionary<string, object> Simplify(this HttpHeaders headers) =>
        headers
            .OrderBy(x => x.Key.ToLowerInvariant())
            .ToDictionary(
                x => x.Key,
                x =>
                {
                    var values = x.Value.ToList();
                    var key = x.Key.ToLowerInvariant();
                    if (key is "date" or "expires" or "last-modified")
                    {
                        if (DateTime.TryParse(values.First(), out var date))
                        {
                            return date;
                        }
                    }

                    return (object)string.Join(",", values);
                });

    public static Dictionary<string, object> NotCookies(this HttpHeaders headers) =>
        headers
            .Simplify()
            .Where(x => x.Key != "Set-Cookie")
            .ToDictionary(x => x.Key, x => x.Value);

    public static Dictionary<string, string?> Cookies(this HttpHeaders headers) =>
        headers
            .Simplify()
            .Where(x => x.Key == "Set-Cookie")
            .Select(x =>
            {
                var stringSegment = (string)x.Value;
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(x => x.Name.Value!, x => x.Value.Value);
}