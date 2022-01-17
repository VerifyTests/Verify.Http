﻿using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;

class UriConverter :
    WriteOnlyJsonConverter<Uri>
{
    public override void WriteJson(
        JsonWriter writer,
        Uri value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (ShouldUseOriginalString(value))
        {
            writer.WriteValue(value.OriginalString);
            return;
        }

        var path = GetPath(value);

        serializer.Serialize(writer,
            new UriWrapper
            {
                Path = path,
                Query = HttpUtility.ParseQueryString(value.Query)
            });
    }

    public static bool ShouldUseOriginalString(Uri value)
    {
        return value.IsAbsoluteUri == false ||
               string.IsNullOrWhiteSpace(value.Query);
    }

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