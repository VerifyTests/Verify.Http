[TestFixture]
public class ResponseCombos
{
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
        [Values] bool dates,
        [Values] bool dupHeader,
        [Values] bool uri)
    {
        var response = HttpBuilder.Response(cookie, version, trailing, content, dates, dupHeader);

        if (request)
        {
            response.RequestMessage = HttpBuilder.Request(auth, content, dates, dupHeader, uri);
        }

        if (nested)
        {
            return Verify(new
            {
                response
            });
        }

        return Verify(response);
    }
}