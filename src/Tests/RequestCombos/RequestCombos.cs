public class RequestCombos
{
    [Test]
    [Explicit]
    public void Purge()
    {
        var path = Path.Combine(ProjectFiles.ProjectDirectory, "RequestCombos");
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
        [Matrix] bool version,
        [Matrix(ContentType.Empty, ContentType.String, ContentType.Image)] ContentType content,
        [Matrix] bool dates,
        [Matrix] bool dupHeader,
        [Matrix] bool uri)
    {
        var request = HttpBuilder.Request(auth, version, content, dates, dupHeader, uri);

        if (nested)
        {
            return Verify(new {request});
        }

        return Verify(request);
    }
}