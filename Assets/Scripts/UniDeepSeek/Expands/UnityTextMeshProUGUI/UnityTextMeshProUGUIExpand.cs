using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Xiyu.UniDeepSeek.Events;

#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.UnityTextMeshProUGUI
{
    public enum ContentFlushCriteria
    {
        ByCharacterCount,
        ByTokenCount
    }


    public static class UnityTextMeshProUGUIExpand
    {
        /// <summary>
        /// 以流式方式显示聊天完成事件，支持推理内容和普通内容的分别处理，并提供内容刷新控制。
        /// <para>（适用于普通的ChatStream）</para>
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="asyncEnumerable">聊天完成数据流</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteria">内容刷新标准，决定如何计算内容长度</param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="flushCriteria"/> 不是有效的枚举值 </exception>
        public static async UniTask DisplayChatStreamBasicAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            int flushThreshold = 10, ContentFlushCriteria flushCriteria = ContentFlushCriteria.ByCharacterCount)
        {
            var stringBuilder = new StringBuilder(512);
            var contentLengthCounter = 0;

            await foreach (var chatCompletion in asyncEnumerable)
            {
                var message = chatCompletion.GetMessage();
                AppendContent(message.Content);
            }

            return;

            // 局部函数：追加内容到缓冲区
            void AppendContent(string content)
            {
                if (string.IsNullOrEmpty(content)) return;

                stringBuilder.Append(content);
                contentLengthCounter += flushCriteria switch
                {
                    ContentFlushCriteria.ByCharacterCount => content.Length,
                    ContentFlushCriteria.ByTokenCount => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(flushCriteria), flushCriteria, null)
                };

                if (contentLengthCounter > flushThreshold)
                    FlushContentToTextMesh();
            }

            // 局部函数：将缓冲区内容推送到TextMeshPro
            void FlushContentToTextMesh()
            {
                if (stringBuilder.Length == 0) return;

                textMeshPro.text += stringBuilder.ToString();
                stringBuilder.Clear();
                contentLengthCounter = 0;
            }
        }

        /// <summary>
        /// 以流式方式显示聊天完成事件，支持推理内容和普通内容的分别处理，并提供内容刷新控制。
        /// <para>（适用于普通的ChatStream）</para>
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteria">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>完成时返回内部使用的 ChatCompletionEvent 实例</returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="flushCriteria"/> 不是有效的枚举值 </exception>
        public static ChatCompletionEvent DisplayChatStreamWithEvents(
            this TMPro.TMP_Text textMeshPro, int flushThreshold = 10,
            ContentFlushCriteria flushCriteria = ContentFlushCriteria.ByCharacterCount)
        {
            var stringBuilder = new StringBuilder(512);
            var contentLengthCounter = 0;

            var chatCompletionEvent = new ChatCompletionEvent();

            chatCompletionEvent.ContentEventSetting.SetUpdate(completion => AppendContent(completion.GetMessage().Content));

            return chatCompletionEvent;

            // 局部函数：追加内容到缓冲区
            void AppendContent(string content)
            {
                if (string.IsNullOrEmpty(content)) return;

                stringBuilder.Append(content);
                contentLengthCounter += flushCriteria switch
                {
                    ContentFlushCriteria.ByCharacterCount => content.Length,
                    ContentFlushCriteria.ByTokenCount => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(flushCriteria), flushCriteria, null)
                };

                if (contentLengthCounter > flushThreshold)
                    FlushContentToTextMesh();
            }

            // 局部函数：将缓冲区内容推送到TextMeshPro
            void FlushContentToTextMesh()
            {
                if (stringBuilder.Length == 0) return;

                textMeshPro.text += stringBuilder.ToString();
                stringBuilder.Clear();
                contentLengthCounter = 0;
            }
        }

        /// <summary>
        /// 以流式方式显示聊天完成事件，支持推理内容和普通内容的分别处理，并提供内容刷新控制和颜色格式化选项。
        /// <para>（适用于深度思考模式）</para>
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="colorHex">推理内容的十六进制颜色代码（如 "#FF5733" 或 "FF5733"，为空时不设置颜色）</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteria">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>完成时返回内部使用的 ChatCompletionEvent 实例</returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="flushCriteria"/> 不是有效的枚举值 </exception>
        public static ChatCompletionEvent DisplayReasoningStreamWithEvents(
            this TMPro.TMP_Text textMeshPro,
            [CanBeNull] string colorHex = null,
            int flushThreshold = 10, ContentFlushCriteria flushCriteria = ContentFlushCriteria.ByCharacterCount)
        {
            var chatCompletionEvent = new ChatCompletionEvent();
            var stringBuilder = new StringBuilder(1024);
            var contentLengthCounter = 0;

            // 配置推理事件处理
            chatCompletionEvent.ReasoningEventSetting
                .SetEnter(completion =>
                {
                    var message = completion.GetMessage();
                    var formattedContent = string.IsNullOrEmpty(colorHex)
                        ? message.ReasoningContent
                        : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
                    AppendContent(formattedContent);
                })
                .SetUpdate(completion => { AppendContent(completion.GetMessage().ReasoningContent); })
                .SetExit(_ =>
                {
                    stringBuilder.Append("</color>\n\n");
                    FlushContentToTextMesh();
                });

            // 配置内容事件处理
            chatCompletionEvent.ContentEventSetting
                .SetEnter(completion => { AppendContent(completion.GetMessage().Content); })
                .SetUpdate(completion => { AppendContent(completion.GetMessage().Content); })
                .SetExit(_ => FlushContentToTextMesh());

            return chatCompletionEvent;

            // 局部函数：追加内容到缓冲区
            void AppendContent(string content)
            {
                if (string.IsNullOrEmpty(content)) return;

                stringBuilder.Append(content);
                contentLengthCounter += flushCriteria switch
                {
                    ContentFlushCriteria.ByCharacterCount => content.Length,
                    ContentFlushCriteria.ByTokenCount => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(flushCriteria), flushCriteria, null)
                };

                if (contentLengthCounter > flushThreshold)
                    FlushContentToTextMesh();
            }

            // 局部函数：将缓冲区内容推送到TextMeshPro
            void FlushContentToTextMesh()
            {
                if (stringBuilder.Length == 0) return;

                textMeshPro.text += stringBuilder.ToString();
                stringBuilder.Clear();
                contentLengthCounter = 0;
            }
        }

        /// <summary>
        /// 以流式方式显示聊天完成事件，支持推理内容和普通内容的分别处理，并提供内容刷新控制和颜色格式化选项。
        /// <para>（适用于深度思考模式）</para>
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="asyncEnumerable">聊天完成数据流</param>
        /// <param name="colorHex">推理内容的十六进制颜色代码（如 "#FF5733" 或 "FF5733"，为空时不设置颜色）</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteria">内容刷新标准，决定如何计算内容长度</param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="flushCriteria"/> 不是有效的枚举值 </exception>
        public static async UniTask DisplayReasoningChatStreamBasicAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            [CanBeNull] string colorHex = null,
            int flushThreshold = 10, ContentFlushCriteria flushCriteria = ContentFlushCriteria.ByCharacterCount)
        {
            var stringBuilder = new StringBuilder(1024);
            var contentLengthCounter = 0;

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
                    var formattedContent = string.IsNullOrEmpty(colorHex)
                        ? message.ReasoningContent
                        : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
                    AppendContent(formattedContent);
                }
                // 处理思考阶段的继续
                else if (isThinking && hasReasoning && !hasContent)
                {
                    AppendContent(message.ReasoningContent);
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
                    stringBuilder.Append(message.Content);
                    FlushContentToTextMesh();
                }
            }


            return;

            // 局部函数：追加内容到缓冲区
            void AppendContent(string content)
            {
                if (string.IsNullOrEmpty(content)) return;

                stringBuilder.Append(content);
                contentLengthCounter += flushCriteria switch
                {
                    ContentFlushCriteria.ByCharacterCount => content.Length,
                    ContentFlushCriteria.ByTokenCount => 1,
                    _ => throw new ArgumentOutOfRangeException(nameof(flushCriteria), flushCriteria, null)
                };

                if (contentLengthCounter > flushThreshold)
                    FlushContentToTextMesh();
            }

            // 局部函数：将缓冲区内容推送到TextMeshPro
            void FlushContentToTextMesh()
            {
                if (stringBuilder.Length == 0) return;

                textMeshPro.text += stringBuilder.ToString();
                stringBuilder.Clear();
                contentLengthCounter = 0;
            }
        }
    }
}