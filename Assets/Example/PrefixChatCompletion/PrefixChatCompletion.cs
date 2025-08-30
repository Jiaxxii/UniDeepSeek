using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.PrefixChatCompletion
{
    public class PrefixChatCompletion : ChatBase
    {
        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("I hate you~baby~"));

            // 哈？我从来没有喜欢过你，你个变态 Zako
            // 如果你的前缀本身就是一句完整的话，那么AI可能不会进行续写，或者直接添加句号。
            const string prefix = "ha? When did I ever like you, ";

            // 接下来Ai将会以prefix为开头，尝试完成剩余的句子
            textMeshProUGUI.text = "chat assistance with chat completion start with prefix: " + prefix;

            var (state, chatCompletion) = await _deepSeekChat.ChatPrefixCompletionAsync(prefix, cancellationToken: cancellationToken);
            if (state == ChatState.Success)
            {
                textMeshProUGUI.text = chatCompletion.GetMessage().Content;
            }
        }
    }
}