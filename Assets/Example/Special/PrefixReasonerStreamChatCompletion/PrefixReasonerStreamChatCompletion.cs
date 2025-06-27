using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.Special.PrefixReasonerStreamChatCompletion
{
    public class PrefixReasonerStreamChatCompletion : MonoBehaviour
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
            try
            {
                var cancellationToken = this.GetCancellationTokenOnDestroy();
                const string prefix = "你好呀，你叫什么呀";
                const string think = "嗯，用户发来“你好呀，你叫什么呀？”,看起来是第一次使用AI。话中出现多次“呀”用户可能是女孩子。对于女孩子我应该不能用太死板的回复。\n\n" +
                                     "女孩子应该都很喜欢猫这种可爱的动物吧？对吧。\n\n" +
                                     "我可以模仿小猫的语气来友好的做出回答，比如使用“喵~”、“肉爪子”、“炸毛”之类的词语进行回复，注意保持礼貌与友好。";

                chatText.text = $"（{think}）\n\n{prefix}";
                await foreach (var chatCompletion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync(
                                   prefix, think,
                                   onCompletion: chatCompletion => completion = chatCompletion,
                                   cancellationToken: cancellationToken))
                {
                    var message = chatCompletion.Choices[0].SourcesMessage;

                    if (!string.IsNullOrEmpty(message.Content))
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