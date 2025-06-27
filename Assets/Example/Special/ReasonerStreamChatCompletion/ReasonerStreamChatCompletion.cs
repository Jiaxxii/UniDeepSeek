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

#if !ODIN_INSPECTOR
        private void Start()
        {
            ReasonerStreamChatCompletionAsync().Forget();
        }
#endif

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("ReasonerStreamChatCompletionAsync")]
#endif
        private async UniTaskVoid ReasonerStreamChatCompletionAsync()
        {
#if ODIN_INSPECTOR
            if (!Application.isPlaying)
            {
                Debug.LogWarning("请在运行时调用`ReasonerStreamChatCompletionAsync`");
                return;
            }
#endif
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
                chatText.text = "（";
                var cancellationToken = this.GetCancellationTokenOnDestroy();
                await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(c => completion = c, cancellationToken))
                {
                    var message = chatCompletion.Choices[0].SourcesMessage;

                    if (string.IsNullOrEmpty(message.Content))
                    {
                        chatText.text += message.ReasoningContent;
                    }
                    else if (think)
                    {
                        think = false;
                        chatText.text += "）\n\n";
                    }

                    if (!think)
                    {
                        chatText.text += message.Content;
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