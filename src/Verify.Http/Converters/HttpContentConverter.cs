using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class HttpContentConverter :
    WriteOnlyJsonConverter<HttpContent>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpContent content,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, content.Headers);

        WriteIfText(writer, content, serializer);

        writer.WriteEndObject();
    }

    static void WriteIfText(JsonWriter writer, HttpContent content, JsonSerializer serializer)
    {
        if (!content.IsText(out var subType))
        {
            return;
        }

        writer.WritePropertyName("Value");
        var result = content.ReadAsString();

        if (subType == "json")
        {
            try
            {
                serializer.Serialize(writer, JToken.Parse(result));
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
                serializer.Serialize(writer, XDocument.Parse(result));
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