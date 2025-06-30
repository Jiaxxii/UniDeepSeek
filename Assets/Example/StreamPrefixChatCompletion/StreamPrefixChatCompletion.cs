using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.StreamPrefixChatCompletion
{
    public class StreamPrefixChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;


        private bool _isRunning;

        private void Start()
        {
            StreamPrefixChatCompletionAsync().Forget();
        }


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("启动", DrawResult = false)]
#endif
        private async UniTaskVoid StreamPrefixChatCompletionAsync()
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

            if (!Application.isPlaying)
                chatText.text = "...";
            try
            {
                const string prefix = "你好呀，西，你可以叫我小深。";

                chatText.text = prefix;
                Xiyu.UniDeepSeek.ChatCompletion completion = null;
                await foreach (var chatCompletion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync(prefix, null, onCompletion: c => completion = c))
                {
                    if (Application.isPlaying)
                        chatText.text += chatCompletion.Choices[0].SourcesMessage.Content;
                    else Debug.Log($"收到消息：{chatCompletion.Choices[0].SourcesMessage.Content}");
                }

                Debug.Log("完整消息：" + completion.Choices[0].SourcesMessage.Content);
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogWarning("StreamChatCompletionAsync canceled: " + exception.Message);
            }

            _isRunning = false;
        }
    }
}