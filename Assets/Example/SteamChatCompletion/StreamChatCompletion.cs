using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.SteamChatCompletion
{
    public class StreamChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;


#if !ODIN_INSPECTOR
        private void Start()
        {
            StreamChatCompletionAsync().Forget();
        }
#endif

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("StreamChatCompletionAsync")]
#endif
        private async UniTaskVoid StreamChatCompletionAsync()
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Messages.Add(new UserMessage("你好，我叫“西”你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            chatText.text = "...";
            try
            {
                Xiyu.UniDeepSeek.ChatCompletion completion = null;
                await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(onCompletion: c => completion = c))
                {
                    chatText.text += chatCompletion.Choices[0].SourcesMessage.Content;
                }

                Debug.Log("完整消息：" + completion.Choices[0].SourcesMessage.Content);
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogWarning("StreamChatCompletionAsync canceled: " + exception.Message);
            }
        }
    }
}