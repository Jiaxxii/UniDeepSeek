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

        private bool _isRunning;

        private void Start()
        {
            PrefixChatCompletionAsync().Forget();
        }


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("启动", DrawResult = false)]
#endif
        private async UniTaskVoid PrefixChatCompletionAsync()
        {
            if (_isRunning)
            {
                Debug.Log("正在运行中，请勿重复启动！");
                return;
            }

            _isRunning = true;

            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            requestParameter.Messages.Add(new UserMessage("你好，我叫“西”你叫什么呀？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            const string prefix = "你好呀，西，你可以叫我小深。";
            Debug.Log($"AI的回答将尝试以“{prefix}”开头");

            var (state, chatCompletion) = await deepSeekChat.ChatPrefixCompletionAsync(prefix: "你好呀，西，你可以叫我小深。");

            if (Application.isPlaying)
            {
                chatText.text = $"request state: <color=#E08E4A>{state}</color>";

                if (state == ChatState.Success)
                    chatText.text = $"{chatCompletion.Choices[0].SourcesMessage.Content}";
            }
            else
            {
                Debug.Log($"request state: {state}");
                if (state == ChatState.Success)
                    Debug.Log($"AI回复: {chatCompletion.Choices[0].SourcesMessage.Content}");
            }


            _isRunning = false;
        }
    }
}