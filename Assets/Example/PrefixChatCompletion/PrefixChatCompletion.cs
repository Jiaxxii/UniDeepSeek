using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.PrefixChatCompletion
{
    public class PrefixChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;

#if !ODIN_INSPECTOR
        private void Start()
        {
            PrefixChatCompletionAsync().Forget();
        }
#endif

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("PrefixChatCompletionAsync")]
#endif
        private async UniTaskVoid PrefixChatCompletionAsync()
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Messages.Add(new UserMessage("你好，我叫“西”你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            const string prefix = "你好呀，西，你可以叫我小深。";
            Debug.Log($"AI的回答将尝试以“{prefix}”开头");

            var (state, chatCompletion) = await deepSeekChat.ChatPrefixCompletionAsync(prefix: "你好呀，西，你可以叫我小深。");

            chatText.text = $"request state: <color=#E08E4A>{state}</color>";

            if (state == ChatState.Success)
            {
                chatText.text = $"{chatCompletion.Choices[0].SourcesMessage.Content}";
            }
        }
    }
}