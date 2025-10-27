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
    [Test]
    public Task Run(
        [Values] bool nested,
        [Values] bool auth,
        [Values] bool cookie,
        [Values] bool request,
        [Values] bool defvaultVersion,
        [Values] bool trailing,
        [Values] ContentType content,
        [Values] bool dateHeaders,
        [Values] bool dupHeader)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = BuildContent(content)
        };

        AddHeaders(dupHeader, dateHeaders, auth, cookie, response.Headers);

        if (trailing)
        {
            AddHeaders(dupHeader, dateHeaders, auth, cookie, response.TrailingHeaders);
        }

        if (defvaultVersion)
        {
            response.Version = HttpExtensions.defaultRequestVersion;
        }
        else
        {
            response.Version = new(0, 1);
        }

        if (request)
        {
            var requestMessage = new HttpRequestMessage
            {
                Content = BuildContent(content)
            };

            AddHeaders(dupHeader, dateHeaders, auth, cookie, requestMessage.Headers);
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

    static void AddHeaders(bool dupHeader, bool dateHeaders, bool auth, bool cookie, HttpHeaders headers)
    {
        if (dupHeader)
        {
            headers.Add("dupHeader", "value1");
            headers.Add("dupHeader", "value2");
        }
        if (dateHeaders)
        {
            headers.Add("date", "2020/05/01");
            headers.Add("expires", "2020/05/02");
            headers.Add("last-modified", "2020/05/03");
        }

        if (auth)
        {
            headers.Add("authorization", "BAD");
        }

        if (cookie)
        {
            headers.Add("Set-Cookie", "BAD");
        }
    }
}