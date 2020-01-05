using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class SignInResultConverter :
    ResultConverter<SignInResult>
{
    protected override void InnerWrite(JsonWriter writer, SignInResult result, JsonSerializer serializer)
    {
        writer.WritePropertyName("AuthenticationScheme");
        serializer.Serialize(writer, result.AuthenticationScheme);
        //TODO: Claims
        //serializer.Serialize(writer, result.Principal.Claims);
        if (result.Properties.Items.Any())
        {
            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, result.Properties.Items);
        }
    }
}