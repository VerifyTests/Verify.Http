static class HttpBuilder
{
    static DateTimeOffset dateHeader = new(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);

    public static void Purge(string name)
    {
        var path = Path.Combine(AttributeReader.GetProjectDirectory(), name);
        foreach (var file in Directory.EnumerateFiles(path, "*.txt"))
        {
            File.Delete(file);
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.png"))
        {
            File.Delete(file);
        }
    }

    public static HttpRequestMessage Request(bool auth, bool version, ContentType content, bool dateHeaders, bool dupHeader, bool uri)
    {
        var request = new HttpRequestMessage
        {
            Content = BuildContent(content)
        };

        if (auth)
        {
            request.Headers.Authorization = new("authScheme", "authParam");
        }

        if (version)
        {
            request.Version = new(0, 1);
            request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        }

        if (uri)
        {
            request.RequestUri = new("https://site/path");
        }

        if (dateHeaders)
        {
            request.Content?.Headers.Expires = dateHeader;
            request.Content?.Headers.LastModified = dateHeader;
            request.Headers.Date = dateHeader;
        }

        AddHeaders(dupHeader, request.Headers);
        return request;
    }

    public static HttpResponseMessage Response(bool cookie, bool version, bool trailing, ContentType content, bool dateHeaders, bool dupHeader)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = BuildContent(content),
        };
        if (dateHeaders)
        {
            // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            response.Content?.Headers.Expires = dateHeader;
            response.Content?.Headers.LastModified = dateHeader;
            // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        }

        AddHeaders(dupHeader, response.Headers);

        if (dateHeaders)
        {
            response.Headers.Date = dateHeader;
        }

        if (cookie)
        {
            response.Headers.Add("Set-Cookie", "sessionId=abc123; Max-Age=3600");
        }

        if (trailing)
        {
            AddHeaders(dupHeader, response.TrailingHeaders);
            if (dateHeaders)
            {
                response.TrailingHeaders.Date = dateHeader;
            }
        }

        if (version)
        {
            response.Version = new(0, 1);
        }

        return response;
    }

    static HttpContent? BuildContent(ContentType content)
    {
        switch (content)
        {
            case ContentType.Empty:
            {
                return null;
            }
            case ContentType.Multi:
            {
                return new MultipartContent
                {
                    new StringContent("some text value"),
                    BuildImageContent()
                };
            }
            case ContentType.String:
                return new StringContent("the content");
            case ContentType.Image:
            {
                return BuildImageContent();
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(content), content, null);
        }
    }

    static StreamContent BuildImageContent()
    {
        var stream = File.OpenRead(EmptyFiles.AllFiles.GetPathFor("png"));
        return new(stream)
        {
            Headers =
            {
                ContentType = new("image/png")
            }
        };
    }

    static void AddHeaders(bool dupHeader, HttpHeaders headers)
    {
        if (dupHeader)
        {
            headers.Add("dupHeader", "value1");
            headers.Add("dupHeader", "value2");
        }
    }
}