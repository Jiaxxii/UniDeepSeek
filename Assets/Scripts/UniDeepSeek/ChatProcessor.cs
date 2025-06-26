using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using JetBrains.Annotations;
using UnityEngine;


namespace Xiyu.UniDeepSeek
{
    public class ChatProcessor
    {
        public ChatProcessor(string apiKey)
        {
            _apiKey = apiKey;
        }

        private readonly string _apiKey;

        public Uri BaseUri { get; private set; } = new("https://api.deepseek.com");

        public string DefaultMediaType { get; set; } = "application/json";


        public async UniTask<string> GetChatCompletionStringAsync([NotNull] string requestUri, [NotNull] string content, CancellationToken? cancellationToken = null)
        {
            using var httpResponseMessage = await GetResponseMessageAsync(requestUri, content, HttpCompletionOption.ResponseContentRead, cancellationToken);

            var readAsString = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(readAsString);
            }

            return readAsString;
        }

        public async UniTask<Stream> GetChatCompletionStreamAsync([NotNull] string requestUri, [NotNull] string content, CancellationToken? cancellationToken = null)
        {
            using var httpResponseMessage = await GetResponseMessageAsync(requestUri, content, HttpCompletionOption.ResponseContentRead, cancellationToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
            }

            return await httpResponseMessage.Content.ReadAsStreamAsync();
        }

        public UniTaskCancelableAsyncEnumerable<string> StreamChatCompletionLinesAsync([NotNull] string requestUri, [NotNull] string content,
            CancellationToken? cancellationToken = null)
        {
            return UniTaskAsyncEnumerable.Create<string>(async (writer, cts) =>
            {
                await using var stream = await SendStreamRequestAsync(requestUri, content, cts);
                using var streamReader = new StreamReader(stream);

                while (await streamReader.ReadLineAsync() is { } line)
                {
                    cts.ThrowIfCancellationRequested();
                    await writer.YieldAsync(line);
                }
            }).WithCancellation(cancellationToken ?? CancellationToken.None);
        }

        public async UniTask<Stream> SendStreamRequestAsync([NotNull] string requestUri, [NotNull] string content, CancellationToken? cancellationToken = null)
        {
            var httpResponseMessage = await GetResponseMessageAsync(requestUri, content, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
            }

            return await httpResponseMessage.Content.ReadAsStreamAsync();
        }

        private UniTask<HttpResponseMessage> GetResponseMessageAsync([NotNull] string requestUri, [NotNull] string content, HttpCompletionOption httpCompletionOption,
            CancellationToken? cancellationToken = null)
        {
            var uri = GetUri(requestUri);
            var stringContent = new StringContent(content, System.Text.Encoding.UTF8, DefaultMediaType);

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequestMessage.Content = stringContent;
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");

            return MainHttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        protected Uri GetUri([NotNull] string requestUri)
        {
            if (string.IsNullOrWhiteSpace(requestUri))
                throw new ArgumentException("Request URI cannot be null or empty.", nameof(requestUri));

            if (BaseUri == null)
                throw new InvalidOperationException("Base URI is not set.");

            return new Uri(BaseUri, requestUri);
        }
    }
}