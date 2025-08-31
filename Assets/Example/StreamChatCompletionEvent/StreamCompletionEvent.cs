using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.Events;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.UnityTextMeshProUGUI;

namespace Example.StreamChatCompletionEvent
{
    public class StreamCompletionEvent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI chatText;


        private void Start()
        {
            StreamChatCompletionAsync().Forget();
        }


        private async UniTaskVoid StreamChatCompletionAsync()
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            // 使用深度思考模型才有意义
            requestParameter.Model = ChatModel.Reasoner;

            requestParameter.Messages.Add(new SystemMessage("Answer me in English without using any emojis"));
            requestParameter.Messages.Add(new UserMessage("Hello, my name is \"Xi\". What's your name?"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");


            var asyncEnumerable = await deepSeekChat.StreamChatCompletionsEnumerableAsync(destroyCancellationToken);

            // 你可以在 Inspector 中设置颜色
            // 这个颜色表示深度思考内容的颜色
            var colorHex = ColorUtility.ToHtmlStringRGB(new Color(1, 0, 1));
            ChatCompletionEvent chatCompletionEvent = chatText.DisplayReasoningStreamWithEvents(colorHex: colorHex);

            // 触发打印
            await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
        }

        private async UniTaskVoid StreamChatCompletionCustomizeAsync()
        {
            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            // 使用深度思考模型才有意义
            requestParameter.Model = ChatModel.Reasoner;

            requestParameter.Messages.Add(new SystemMessage("Answer me in English without using any emojis"));
            requestParameter.Messages.Add(new UserMessage("Hello, my name is \"Xi\". What's your name?"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");


            var asyncEnumerable = await deepSeekChat.StreamChatCompletionsEnumerableAsync(destroyCancellationToken);

            // 你可以在 Inspector 中设置颜色
            // 这个颜色表示深度思考内容的颜色
            var colorHex = ColorUtility.ToHtmlStringRGB(new Color(1, 0, 1));

            // 你可以可以直接通过构造函数来创建自定义的 ChatCompletionEvent

            // 如果有需要，你可以继承 `IChatCompletionRunning` 接口以改变工作方式
            var chatCompletionEvent = new ChatCompletionEvent();

            chatCompletionEvent.ReasoningEventSetting
                // 开始接收到深度思考内容时的事件
                // Set 开头的方法会覆盖委托，如果需要追加请使用 Append 开头的方法
                .SetEnter(completion =>
                {
                    var message = completion.GetMessage();
                    chatText.text = string.IsNullOrEmpty(colorHex)
                        ? message.ReasoningContent
                        : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
                })
                // 接收到内容时（不包含第一条）
                .SetUpdate(completion => chatText.text += completion.GetMessage().ReasoningContent)
                // 结束接收时（completion与最后一条相同，即最后一次更新）
                .SetExit(_ => chatText.text += "</color>\n\n");


            chatCompletionEvent.ContentEventSetting.SetEnter(completion => chatText.text += completion.GetMessage().Content)
                .SetUpdate(completion => chatText.text += completion.GetMessage().Content);

            await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);

            // 这种操作与上面的 `await chatText.DisplayChatStreamWithEventsAsync(asyncEnumerable, colorHex: colorHex)` 是等价的。
            // `DisplayChatStreamWithEventsAsync` 只有对 TMPro.TMP_Text 的拓展（你也应该使用TextMeshPro，因为Text已经过时）
        }
    }
}