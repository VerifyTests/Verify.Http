public class CallCombos
{
    [Test]
    [Explicit]
    public void Purge()
    {
        var path = Path.Combine(ProjectFiles.ProjectDirectory, "CallCombos");
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
        [Matrix] bool version,
        [Matrix] bool trailing,
        [Matrix(ContentType.Empty, ContentType.String, ContentType.Image)] ContentType content,
        [Matrix] bool dates,
        [Matrix] bool dupHeader,
        [Matrix] bool uri)
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