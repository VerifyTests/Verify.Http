class HttpContentConverter:
    WriteOnlyJsonConverter<HttpContent>
{
    public override void Write(VerifyJsonWriter writer, HttpContent content)
    {
        writer.WriteStartObject();

        writer.WriteMember(content, content.Headers.Simplify(), "Headers");

        WriteIfText(writer, content);

        writer.WriteEndObject();
    }

    static void WriteIfText(VerifyJsonWriter writer, HttpContent content)
    {
        if (!content.IsText(out var subType))
        {
            if (writer.HasHttpTextResponseScrubber())
            {
                throw new("Content is not text, but scrubContent is set.");
            }

            return;
        }

        writer.WritePropertyName("Value");
        var result = content.ReadAsString();
        if (writer.TryGetHttpTextResponseScrubber(out var scrubContent))
        {
            result = scrubContent(result);
        }

        if (subType == "json")
        {
            try
            {
                writer.Serialize(JToken.Parse(result));
            }
            catch
            {
                writer.WriteValue(result);
            }
        }
        else if (subType == "xml")
        {
            try
            {
                writer.Serialize(XDocument.Parse(result));
            }
            catch
            {
                writer.WriteValue(result);
            }
        }
        else
        {
            writer.WriteValue(result);
        }
    }
}