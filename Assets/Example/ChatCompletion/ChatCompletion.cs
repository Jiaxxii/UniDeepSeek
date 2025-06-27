using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.ChatCompletion
{
    public class ChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;

#if !ODIN_INSPECTOR
        private void Start()
        {
            ChatCompletionAsync().Forget();
        }
#endif

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("Chat Completion Async")]
#endif
        private async UniTaskVoid ChatCompletionAsync()
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Messages.Add(new UserMessage("你好，我叫“西”你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");
            
            var (state, chatCompletion) = await deepSeekChat.ChatCompletionAsync();

            chatText.text = $"request state: <color=#E08E4A>{state}</color>";

            if (state == ChatState.Success)
            {
                chatText.text = $"{chatCompletion.Choices[0].SourcesMessage.Content}";
            }
        }
    }
}