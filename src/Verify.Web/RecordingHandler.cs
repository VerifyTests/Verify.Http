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

        public void Resume()
        {
            Recording = true;
        }

        public void Pause()
        {
            Recording = false;
        }

        public RecordingHandler(bool recording = true)
        {
            Recording = recording;
        }

        public bool Recording { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
        {
            if (!Recording)
            {
                return await base.SendAsync(request, cancellation);
            }
            string? requestText = null;
            var requestContent = request.Content;
            if (requestContent != null)
            {
                if (requestContent.IsText())
                {
                    requestText = await requestContent.ReadAsStringAsync();
                }
            }

            var response = await base.SendAsync(request, cancellation);

            var responseContent = response.Content;
            string? responseText = null;
            if (responseContent.IsText())
            {
                responseText = await responseContent.ReadAsStringAsync();
            }

            var item = new LoggedSend(
                request.RequestUri,
#if(NET5_0_OR_GREATER)
                request.Options,
#endif
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