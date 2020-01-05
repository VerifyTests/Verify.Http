using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

class EmptyResultConverter :
    ResultConverter<EmptyResult>
{
    protected override void InnerWrite(JsonWriter writer, EmptyResult result, JsonSerializer serializer)
    {
    }
}