[TestFixture]
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