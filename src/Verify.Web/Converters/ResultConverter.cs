using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;
using Microsoft.AspNetCore.Mvc;

abstract class ResultConverter<T> :
    WriteOnlyJsonConverter<T>
    where T : ActionResult
{
    public override void WriteJson(JsonWriter writer, T? result, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (result == null)
        {
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("ResultType");
        serializer.Serialize(writer, result.GetType().Name);
        InnerWrite(writer, result, serializer);
        writer.WriteEndObject();
    }

    protected abstract void InnerWrite(JsonWriter writer, T result, JsonSerializer serializer);
}