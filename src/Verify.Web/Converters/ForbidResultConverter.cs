using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ForbidResultConverter :
    ResultConverter<ForbidResult>
{
    protected override void InnerWrite(JsonWriter writer, ForbidResult result, JsonSerializer serializer)
    {
        if (result.AuthenticationSchemes.Count == 1)
        {
            writer.WritePropertyName("Scheme");
            serializer.Serialize(writer, result.AuthenticationSchemes.Single());
        }
        else
        {
            writer.WritePropertyName("Schemes");
            serializer.Serialize(writer, result.AuthenticationSchemes);
        }
        if (result.Properties.Items.Any())
        {
            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, result.Properties.Items);
        }
    }
}