using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.UniDeepSeek.Events;

#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.UnityTextMeshProUGUI
{
    public static class UnityTextMeshProUGUIExpand
    {
        public static async UniTask<ChatCompletionEvent> DisplayChatStreamWithEventsAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            bool startClear = true,
            [CanBeNull] string colorHex = null)
        {
            if (startClear) textMeshPro.text = string.Empty;

            var chatCompletionEvent = new ChatCompletionEvent();
            chatCompletionEvent.ReasoningEventSetting
                .SetEnter(completion =>
                {
                    var message = completion.GetMessage();
                    textMeshPro.text = string.IsNullOrEmpty(colorHex)
                        ? message.ReasoningContent
                        : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
                })
                .SetUpdate(completion => textMeshPro.text += completion.GetMessage().ReasoningContent)
                .SetExit(_ => textMeshPro.text += "</color>\n\n");


            chatCompletionEvent.ContentEventSetting.SetEnter(completion => textMeshPro.text += completion.GetMessage().Content)
                .SetUpdate(completion => textMeshPro.text += completion.GetMessage().Content);

            await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
            return chatCompletionEvent;
        }

        public static async UniTask DisplayChatStreamBasicAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            bool startClear = true,
            [CanBeNull] string colorHex = null)
        {
            if (textMeshPro == null)
                throw new ArgumentNullException(nameof(textMeshPro));

            if (startClear)
                textMeshPro.text = string.Empty;

            var isThinking = false;
            await foreach (var completion in asyncEnumerable)
            {
                var message = completion.GetMessage();
                var hasReasoning = !string.IsNullOrEmpty(message.ReasoningContent);
                var hasContent = !string.IsNullOrEmpty(message.Content);

                // 处理思考阶段的开始
                if (!isThinking && hasReasoning && !hasContent)
                {
                    isThinking = true;
                    textMeshPro.text = string.IsNullOrEmpty(colorHex)
                        ? message.ReasoningContent
                        : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
                }
                // 处理思考阶段的继续
                else if (isThinking && hasReasoning && !hasContent)
                {
                    textMeshPro.text += message.ReasoningContent;
                }
                // 处理思考结束，开始回答
                else if (isThinking && !hasReasoning && hasContent)
                {
                    isThinking = false;
                    textMeshPro.text += $"</color>\n\n{message.Content}";
                }
                // 处理直接回答（无思考阶段）
                else if (!isThinking && hasContent)
                {
                    textMeshPro.text += message.Content;
                }
            }

            // 确保在结束时闭合标签（如果思考阶段未正常结束）
            if (isThinking && !string.IsNullOrEmpty(colorHex))
            {
                textMeshPro.text += "</color>";
            }
        }
    }
}