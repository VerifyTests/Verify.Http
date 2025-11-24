using Microsoft.Net.Http.Headers;

static class Extensions
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
                _ => _.Key, object (_) =>
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
}