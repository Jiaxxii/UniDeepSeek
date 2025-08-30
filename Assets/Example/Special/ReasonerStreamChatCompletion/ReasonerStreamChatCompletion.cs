using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using TMPro;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.Events;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.UnityTextMeshProUGUI;

namespace Example.Special.ReasonerStreamChatCompletion
{
    /// <summary>
    /// 在这个示例中，我将展示使用`deepseek-reasoner`模型，并通过流式方法进行请求
    /// </summary>
    public class ReasonerStreamChatCompletion : ChatBase
    {
        private bool _running;

        [SerializeField] private Color reasoningColor = Color.grey;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("@chatCompletion.ID != \"\"")]
#endif
        [SerializeField]
        private ChatCompletion chatCompletion;

        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            _running = true;
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("hi,my name is player!"));
            requestParameter.Model = ChatModel.Reasoner;

            var asyncEnumerable = _deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

            textMeshProUGUI.text = string.Empty;

            // 这几乎等价下面的 `ReasonerStreamChatCompletionAsync` 方法
            await textMeshProUGUI.DisplayReasoningChatStreamBasicAsync(asyncEnumerable, colorHex: ColorToHex(reasoningColor));
            _running = false;
        }

#if ODIN_INSPECTOR
        private bool IsPlaying => Application.isPlaying;

        [Sirenix.OdinInspector.ShowIf("@IsPlaying && !_running")]
        [Sirenix.OdinInspector.Button(DrawResult = false)]
#endif
        protected async UniTaskVoid StartForgetWithEvents()
        {
            if (_running)
            {
                Debug.LogWarning("请等待AI完成回复后调用！");
                return;
            }

            _running = true;
            requestParameter.Messages.Clear();
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("hi,my name is player!"));
            requestParameter.Model = ChatModel.Reasoner;

            var asyncEnumerable = _deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, destroyCancellationToken);

            textMeshProUGUI.text = string.Empty;

            // 如果你需要添加自己的事件处理，可以使用 `DisplayReasoningStreamWithEventsAsync` 方法，他会返回一个 `ChatCompletionEvent` 对象，你可以通过它来添加自己的事件处理
            var chatCompletionEvent = await textMeshProUGUI.DisplayReasoningStreamWithEventsAsync(asyncEnumerable);

            chatCompletionEvent.ReasoningEventSetting
                .AppendEnter(_ => Debug.Log("深度思考开始"))
                .AppendExit(_ => Debug.Log("深度思考结束"));

            chatCompletionEvent.ContentEventSetting.AppendExit(_ => Debug.Log("回答结束"));
            _running = false;
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("@IsPlaying && !_running")]
        [Sirenix.OdinInspector.Button(DrawResult = false)]
#endif
        protected async UniTaskVoid StartForgetWithCustomHandlers()
        {
            if (_running)
            {
                Debug.LogWarning("请等待AI完成回复后调用！");
                return;
            }

            _running = true;
            requestParameter.Messages.Clear();
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("hi,my name is player!"));
            requestParameter.Model = ChatModel.Reasoner;

            var asyncEnumerable = _deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, destroyCancellationToken);

            textMeshProUGUI.text = string.Empty;

            var chatCompletionEvent = new ChatCompletionEvent();

            // 配置推理事件处理
            chatCompletionEvent.ReasoningEventSetting
                .SetEnter(completion =>
                {
                    var message = completion.GetMessage();
                    textMeshProUGUI.text = $"<color=#ff00ff>{message.ReasoningContent}";
                })
                .SetUpdate(completion => textMeshProUGUI.text += completion.GetMessage().ReasoningContent)
                .SetExit(_ => textMeshProUGUI.text += "</color>\n\n");

            // 配置内容事件处理
            chatCompletionEvent.ContentEventSetting
                .SetEnter(completion => textMeshProUGUI.text += completion.GetMessage().Content)
                .SetUpdate(completion => textMeshProUGUI.text += completion.GetMessage().Content);

            chatCompletionEvent.ContentEventSetting.AppendExit(_ => Debug.Log("回答结束"));

            // 触发打印
            await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
            _running = false;
        }


        // 如果你不嫌麻烦，仍然可以自己来分辨深度思考内容和回答内容
        [Obsolete("If you don't mind the trouble, you can still distinguish between deep thinking content and answer content by yourself")]
        private static async UniTaskVoid ReasonerStreamChatCompletionAsync(TextMeshProUGUI chatText, CancellationToken token)
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Model = ChatModel.Reasoner;
            requestParameter.Messages.Add(new UserMessage("你好呀，你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            ChatCompletion completion = null;
            var think = true;
            try
            {
                if (Application.isPlaying)
                    chatText.text = "（";

                await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(c => completion = c, token))
                {
                    var message = chatCompletion.Choices[0].SourcesMessage;

                    if (string.IsNullOrEmpty(message.Content))
                    {
                        if (Application.isPlaying)
                            chatText.text += message.ReasoningContent;
                        else Debug.Log(message.ReasoningContent);
                    }
                    else if (think)
                    {
                        think = false;
                        if (Application.isPlaying)
                            chatText.text += "）\n\n";
                    }

                    if (!think)
                    {
                        if (Application.isPlaying)
                            chatText.text += message.Content;
                        else Debug.Log(message.Content);
                    }
                }

                Debug.Log($"完整消息：{completion.Choices[0].SourcesMessage.Content}\n{completion.Choices[0].SourcesMessage.ReasoningContent}");
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogWarning("chat stream canceled: " + exception.Message);
            }
        }
    }
}