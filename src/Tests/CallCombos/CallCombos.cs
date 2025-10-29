[TestFixture]
public class CallCombos
{
    [Test]
    [Explicit]
    public void Purge() =>
        HttpBuilder.Purge("CallCombos");

    [Test]
    public Task Run(
        [Values] bool nested,
        [Values] bool auth,
        [Values] bool cookie,
        [Values] bool version,
        [Values] bool trailing,
        [Values] ContentType content,
        [Values] bool dates,
        [Values] bool dupHeader,
        [Values] bool uri)
    {
        var response = HttpBuilder.Response(cookie, version, trailing, content, dates, dupHeader);

        var request = HttpBuilder.Request(auth, version, content, dates, dupHeader, uri);

        var call = new HttpCall(request, response);
        if (nested)
        {
            return Verify(new {call});
        }

        return Verify(call);
    }
}