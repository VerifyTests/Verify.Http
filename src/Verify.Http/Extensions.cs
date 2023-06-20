using Microsoft.Net.Http.Headers;

static class Extensions
{
#if(NET48)
    internal static Stream ReadAsStream(this HttpContent content) =>
        content.ReadAsStreamAsync().GetAwaiter().GetResult();
#endif

    internal static string ReadAsString(this HttpContent content) =>
        content.ReadAsStringAsync().GetAwaiter().GetResult();

    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers) =>
        headers
            .ToDictionary(_ => _.Key, _ => string.Join("|", _.Value));

    public static Dictionary<string, object> Simplify(this HttpHeaders headers) =>
        headers
            .OrderBy(_ => _.Key.ToLowerInvariant())
            .ToDictionary(
                _ => _.Key,
                _ =>
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

                    return (object)string.Join(",", values);
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
            .Select(x =>
            {
                var stringSegment = (string)x.Value;
                return SetCookieHeaderValue.Parse(stringSegment);
            })
            .ToDictionary(_ => _.Name.Value!, _ => _.Value.Value);
}