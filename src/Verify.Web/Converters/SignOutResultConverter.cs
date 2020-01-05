using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class SignOutResultConverter :
    ResultConverter<SignOutResult>
{
    protected override void InnerWrite(JsonWriter writer, SignOutResult result, JsonSerializer serializer)
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
        //TODO: Claims
        //serializer.Serialize(writer, result.Principal.Claims);
        if (result.Properties.Items.Any())
        {
            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, result.Properties.Items);
        }
    }
}