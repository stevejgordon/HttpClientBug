using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HttpContextExample.Middleware
{
    public class RefreshJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public RefreshJwtMiddleware(RequestDelegate next, HttpClient httpClient)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            _next = next;
            _httpClient = httpClient;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = "1234";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:12345");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseModel = JsonConvert.DeserializeObject<ResponseContainerModel>(payload);

                    if (!string.IsNullOrEmpty(responseModel?.Result))
                    {
                        token = responseModel.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                // should be an exception here but ex is null!
            }

            context.Response.OnStarting(() =>
            {
                context
                    .Response
                    .Headers
                    .Add(HttpRequestHeader.Authorization.ToString(),
                        new[] { new AuthenticationHeaderValue("Bearer", token).ToString() });

                return Task.CompletedTask;
            });


            await _next.Invoke(context);
        }

        private class ResponseContainerModel
        {
            public string Status { get; set; }
            public string Result { get; set; }
        }
    }
}
