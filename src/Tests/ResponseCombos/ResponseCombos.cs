public class ResponseCombos
{
    [Test]
    [Explicit]
    public void Purge()
    {
        var path = Path.Combine(ProjectFiles.ProjectDirectory, "ResponseCombos");
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
    [MatrixDataSource]
    public Task Run(
        [Matrix] bool nested,
        [Matrix] bool auth,
        [Matrix] bool cookie,
        [Matrix] bool request,
        [Matrix] bool version,
        [Matrix] bool trailing,
        [Matrix(ContentType.Empty, ContentType.String, ContentType.Image)] ContentType content,
        [Matrix] bool dates,
        [Matrix] bool dupHeader,
        [Matrix] bool uri)
    {
        var response = HttpBuilder.Response(cookie, version, trailing, content, dates, dupHeader);

        if (request)
        {
            response.RequestMessage = HttpBuilder.Request(auth, version, content, dates, dupHeader, uri);
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