﻿using System;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.UniDeepSeek
{
    public static class MainHttpClient
    {
        private static readonly HttpClient Client = new()
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        public static UniTask<HttpResponseMessage> GetAsync(string requestUri,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken? cancellationToken = null)
        {
            return Client.GetAsync(requestUri, completionOption, cancellationToken ?? CancellationToken.None).AsUniTask();
        }

        public static UniTask<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken? cancellationToken = null)
        {
            return Client.PostAsync(requestUri, content, cancellationToken ?? CancellationToken.None).AsUniTask();
        }

        public static UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            CancellationToken? cancellationToken = null)
        {
            return Client.SendAsync(request, completionOption, cancellationToken ?? CancellationToken.None)
                .AsUniTask();
        }

        public static void Dispose()
        {
            Client?.Dispose();
        }


//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//         private static void Initialize()
//         {
// #if UNITY_EDITOR
//             var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
//             Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
// #else
//             throw new System.Exception("DeepSeek API key is not set in the project settings.");
// #endif
//         }
    }
}