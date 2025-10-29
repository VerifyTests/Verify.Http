[TestFixture]
public class RequestCombos
{
    [Test]
    [Explicit]
    public void Purge() =>
        HttpBuilder.Purge("RequestCombos");

    [Test]
    public Task Run(
        [Values] bool nested,
        [Values] bool auth,
        [Values] bool version,
        [Values] ContentType content,
        [Values] bool dates,
        [Values] bool dupHeader,
        [Values] bool uri)
    {
        var request = HttpBuilder.Request(auth, version, content, dates, dupHeader, uri);

        if (nested)
        {
            return Verify(new {request});
        }

        return Verify(request);
    }
}