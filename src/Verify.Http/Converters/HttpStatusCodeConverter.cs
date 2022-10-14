using System.Net;

class HttpStatusCodeConverter :
    WriteOnlyJsonConverter<HttpStatusCode>
{
    public override void Write(VerifyJsonWriter writer, HttpStatusCode code) =>
        writer.WriteValue($"{code} {(int) code}");
}