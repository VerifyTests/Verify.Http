using System.Net.Http.Headers;

[TestFixture]
public class ResponseCombos
{
    [Test]
    public Task Run(
        [Values] bool nested,
        [Values] bool auth,
        [Values] bool cookie,
        [Values] bool request,
        [Values] bool defvaultVersion,
        [Values] bool trailing,
        [Values] bool content,
        [Values] bool dateHeaders,
        [Values] bool dupHeader)
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);

        AddHeaders(dupHeader, dateHeaders, auth, cookie, response.Headers);

        if (content)
        {
            response.Content = new StringContent("the content");
        }

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
            response.Version = new Version(0, 1);
        }

        if (request)
        {
            var requestMessage = new HttpRequestMessage();
            if (content)
            {
                requestMessage.Content = new StringContent("the request content");
            }

            AddHeaders(dupHeader, dateHeaders, auth, cookie, requestMessage.Headers);
            response.RequestMessage = requestMessage;
        }

        if (nested)
        {
            return Verify(new {response});
        }

        return Verify(response);
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