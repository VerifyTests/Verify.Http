using System.Net.Http.Headers;

[TestFixture]
public class ResponseCombos
{
    public enum ContentType
    {
        Empty,
        String,
        Image
    }

    static DateTimeOffset dateHeader = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);

    [Test]
    [Explicit]
    public void Purge()
    {
        var path = Path.Combine(AttributeReader.GetProjectDirectory(), "ResponseCombos");
        foreach (var file in Directory.EnumerateFiles(path, "*.txt"))
        {
            File.Delete(file);
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.png"))
        {
            File.Delete(file);
        }
    }

    [Test]
    public Task Run(
        [Values] bool nested,
        [Values] bool auth,
        [Values] bool cookie,
        [Values] bool request,
        [Values] bool version,
        [Values] bool trailing,
        [Values] ContentType content,
        [Values] bool dateHeaders,
        [Values] bool dupHeader,
        [Values] bool uri)
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

        if (request)
        {
            var requestMessage = new HttpRequestMessage
            {
                Content = BuildContent(content)
            };

            requestMessage.Headers.Authorization = new("authScheme", "authParam");
            if (uri)
            {
                requestMessage.RequestUri = new("https://site/path");
            }

            if (dateHeaders)
            {
                requestMessage.Content?.Headers.Expires = dateHeader;
                requestMessage.Content?.Headers.LastModified = dateHeader;
            }

            AddHeaders(dupHeader, requestMessage.Headers);
            requestMessage.Headers.Date = dateHeader;
            response.RequestMessage = requestMessage;
        }

        if (nested)
        {
            return Verify(new {response});
        }

        return Verify(response);
    }

    static HttpContent? BuildContent(ContentType content)
    {
        switch (content)
        {
            case ContentType.Empty:
            {
                return null;
            }
            case ContentType.String:
                return new StringContent("the content");
            case ContentType.Image:
            {
                var stream = File.OpenRead(EmptyFiles.AllFiles.GetPathFor("png"));
                return new StreamContent(stream)
                {
                    Headers =
                    {
                        ContentType = new("image/png")
                    }
                };
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(content), content, null);
        }
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