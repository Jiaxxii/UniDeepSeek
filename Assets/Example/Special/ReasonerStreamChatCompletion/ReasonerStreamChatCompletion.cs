using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.Special.ReasonerStreamChatCompletion
{
    /// <summary>
    /// 在这个示例中，我将展示使用`deepseek-reasoner`模型，并通过流式方法进行请求
    /// </summary>
    public class ReasonerStreamChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;

        private bool _isRunning;

        private void Start()
        {
            ReasonerStreamChatCompletionAsync().Forget();
        }


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("启动", DrawResult = false)]
#endif
        private async UniTaskVoid ReasonerStreamChatCompletionAsync()
        {
            if (_isRunning)
            {
                Debug.Log("正在运行中，请等待结束");
                return;
            }

            _isRunning = true;

            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Model = ChatModel.Reasoner;
            requestParameter.Messages.Add(new UserMessage("你好呀，你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            Xiyu.UniDeepSeek.ChatCompletion completion = null;
            var think = true;
            try
            {
                if (Application.isPlaying)
                    chatText.text = "（";

                var cancellationToken = this.GetCancellationTokenOnDestroy();
                await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(c => completion = c, cancellationToken))
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

            _isRunning = false;
        }
    }
}