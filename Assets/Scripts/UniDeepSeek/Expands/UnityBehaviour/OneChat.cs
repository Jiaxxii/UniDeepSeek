using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Xiyu.UniDeepSeek.UnityBehaviour
{
    public static class OneChat
    {
        public static async UniTask<ChatCompletion> OneChatCompletionAsync(this MonoBehaviour behaviour,
            string apiKey,
            string content,
            string systemPrompt = null)
        {
            var builder = DeepSeekChat.Create();

            if (!string.IsNullOrWhiteSpace(systemPrompt))
                builder.Message.AddSystemMessage(systemPrompt);

            var deepSeekChat = builder.Message.AddUserMessage(content)
                .Base.Build(apiKey);

            var (_, chatCompletion) = await deepSeekChat.ChatCompletionAsync(behaviour.destroyCancellationToken);
            return chatCompletion;
        }


        public static IEnumerator OneChatCompletionCoroutine(this MonoBehaviour behaviour,
            string apiKey,
            string content,
            Action<ChatCompletion> onComplete,
            string systemPrompt = null)
        {
            return OneChatCompletionAsync(behaviour, apiKey, content, systemPrompt).ToCoroutine(onComplete);
        }
    }
}