using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VerifyTests.Web
{
    public class RecordingHandler :
        DelegatingHandler
    {
        public ConcurrentQueue<LoggedSend> Sends = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
        {
            string? requestText = null;
            var requestContent = request.Content;
            if (requestContent != null)
            {
                if (requestContent.IsText())
                {
                    requestText = await requestContent.ReadAsStringAsync(cancellation);
                }
            }

            var response = await base.SendAsync(request, cancellation);

            var responseContent = response.Content;
            string? responseText = null;
            if (responseContent.IsText())
            {
                responseText = await responseContent.ReadAsStringAsync(cancellation);
            }

            var item = new LoggedSend(
                request.RequestUri,
                request.Options,
                request.Method.ToString(),
                request.Headers.ToDictionary(),
                requestText,
                response.StatusCode,
                response.Headers.ToDictionary(),
                responseText);
            Sends.Enqueue(item);

            return response;
        }
    }
}