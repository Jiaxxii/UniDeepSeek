using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using JetBrains.Annotations;

namespace Xiyu.UniDeepSeek
{
    public class ChatProcessor
    {
        /// <summary>
        /// 使用指定的API密钥初始化ChatProcessor
        /// </summary>
        /// <param name="apiKey">DeepSeek API密钥</param>
        public ChatProcessor(string apiKey)
        {
            _apiKey = apiKey;
        }

        private readonly string _apiKey;

        /// <summary>
        /// 获取API的基础URI
        /// </summary>
        public Uri BaseUri { get; } = new("https://api.deepseek.com");

        /// <summary>
        /// 设置或获取HTTP请求的默认媒体类型
        /// </summary>
        public string DefaultMediaType { get; set; } = "application/json";

        /// <summary>
        /// 获取聊天补全的字符串响应
        /// </summary>
        /// <param name="requestUri">API请求路径</param>
        /// <param name="content">JSON格式的请求内容</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>包含完整响应的字符串</returns>
        /// <exception cref="HttpRequestException">当HTTP状态码表示失败时抛出</exception>
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

        /// <summary>
        /// 获取聊天补全的流式响应
        /// </summary>
        /// <param name="requestUri">API请求路径</param>
        /// <param name="content">JSON格式的请求内容</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>包含流式响应的Stream对象</returns>
        /// <exception cref="HttpRequestException">当HTTP状态码表示失败时抛出</exception>
        public async UniTask<Stream> GetChatCompletionStreamAsync([NotNull] string requestUri, [NotNull] string content, CancellationToken? cancellationToken = null)
        {
            using var httpResponseMessage = await GetResponseMessageAsync(requestUri, content, HttpCompletionOption.ResponseContentRead, cancellationToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(await httpResponseMessage.Content.ReadAsStringAsync());
            }

            return await httpResponseMessage.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// 创建可取消的聊天补全行流
        /// </summary>
        /// <param name="requestUri">API请求路径</param>
        /// <param name="content">JSON格式的请求内容</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>可异步枚举的字符串序列</returns>
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

        /// <summary>
        /// 发送流式请求并获取响应流
        /// </summary>
        /// <param name="requestUri">API请求路径</param>
        /// <param name="content">JSON格式的请求内容</param>
        /// <param name="cancellationToken">可选的取消令牌</param>
        /// <returns>包含响应数据的Stream</returns>
        /// <exception cref="HttpRequestException">当HTTP状态码表示失败时抛出</exception>
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
            
            return MainHttpClient.SendAsync(httpRequestMessage, httpCompletionOption, cancellationToken);
        }

        /// <summary>
        /// 根据基础URI和请求路径构造完整URI
        /// </summary>
        /// <param name="requestUri">API请求路径</param>
        /// <returns>组合后的完整URI</returns>
        /// <exception cref="ArgumentException">当requestUri为空时抛出</exception>
        /// <exception cref="InvalidOperationException">当BaseUri未设置时抛出</exception>
        protected virtual Uri GetUri([NotNull] string requestUri)
        {
            if (string.IsNullOrWhiteSpace(requestUri))
                throw new ArgumentException("Request URI cannot be null or empty.", nameof(requestUri));

            if (BaseUri == null)
                throw new InvalidOperationException("Base URI is not set.");

            return new Uri(BaseUri, requestUri);
        }
    }
}