static class HttpResponseSplitterResult
{
    public static ConversionResult Convert(HttpResponseMessage instance)
    {
        var targets = new List<Target>();
        var content = instance.Content;
        if (content.TryGetExtension(out var extension))
        {
            if (FileExtensions.IsTextExtension(extension))
            {
                return new(instance, targets);
            }

            targets.Add(new(extension, content.ReadAsStream()));
        }

        return new(
            new
            {
                Response = instance,
                Request = instance.RequestMessage
            },
            targets);
    }
}