using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class ChallengeResultConverter :
    ResultConverter<ChallengeResult>
{
    protected override void InnerWrite(JsonWriter writer, ChallengeResult result, JsonSerializer serializer)
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

        writer.WritePropertyName("Properties");
        serializer.Serialize(writer, result.Properties.Items);
    }
}