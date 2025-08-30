using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using TMPro;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.UnityTextMeshProUGUI;

namespace Example.Special.PrefixReasonerStreamChatCompletion
{
    public class PrefixReasonerStreamChatCompletion : ChatBase
    {
        [SerializeField] private Color reasoningColor = Color.grey;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("@chatCompletion.ID != \"\"")] [SerializeField]
        private ChatCompletion chatCompletion;
#endif
        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            systemPrompt = null;
            requestParameter.Messages.Add(new UserMessage("hello, what's your name?"));

            requestParameter.Model = ChatModel.Reasoner;

            const string prefix = "hello Meow~";
            const string think =
                "Hmm, the user sent 'Hello there, what's your name?'. Looks like it's their first time using an AI. The repeated use of the particle 'ya' suggests the user might be a girl. For a girl, I probably shouldn't respond too formally.\n\n" +
                "I bet most people, especially girls, like cute animals like cats, right?\n\n" +
                "I can imitate a kitten's tone to give a friendly response—using words like 'Meow~', 'paws', or 'fluffy fur' to keep it cute and engaging. Make sure to stay polite and friendly.";

            var asyncEnumerable = _deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync(prefix, think,
                onCompletion: completion => chatCompletion = completion,
                cancellationToken: cancellationToken);

            textMeshProUGUI.text = $"<color={ColorToHex(reasoningColor)}>{think}</color>\n\n{prefix}";

            // 虽然使用的是 Reasoner 模式，但是思考链已经被指定了，AI不会生成新的思考链，我们可以直接使用普通的 `ChatStreamBasic` 拓展方法来完成对话。
            await textMeshProUGUI.DisplayChatStreamBasicAsync(asyncEnumerable);

            Debug.Log($"完整消息:{chatCompletion.GetMessage().Content}");
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

            Xiyu.UniDeepSeek.ChatCompletion completion = null;
            try
            {
                const string prefix = "你好呀，你叫什么呀";
                const string think = "嗯，用户发来“你好呀，你叫什么呀？”,看起来是第一次使用AI。话中出现多次“呀”用户可能是女孩子。对于女孩子我应该不能用太死板的回复。\n\n" +
                                     "女孩子应该都很喜欢猫这种可爱的动物吧？对吧。\n\n" +
                                     "我可以模仿小猫的语气来友好的做出回答，比如使用“喵~”、“肉爪子”、“炸毛”之类的词语进行回复，注意保持礼貌与友好。";

                if (Application.isPlaying)
                    chatText.text = $"（{think}）\n\n{prefix}";
                else Debug.Log($"（{think}）\n\n{prefix}");
                await foreach (var chatCompletion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync(
                                   prefix, think,
                                   onCompletion: chatCompletion => completion = chatCompletion,
                                   cancellationToken: token))
                {
                    var message = chatCompletion.Choices[0].SourcesMessage;

                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        if (Application.isPlaying)
                            chatText.text += message.Content;
                        else Debug.Log($"收到消息：{message.Content}");
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