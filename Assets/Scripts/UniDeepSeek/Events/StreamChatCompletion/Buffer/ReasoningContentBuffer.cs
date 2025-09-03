using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer
{
    public class ReasoningContentBuffer : ContentBuffer, IAggregationStreamHandler
    {
        public ReasoningContentBuffer(Action<string> onFinish, ReasoningOption option = null, int bufferSize = 1024)
            : base(onFinish, option, bufferSize)
        {
            _option = option ?? new ReasoningOption();
        }

        private readonly ReasoningOption _option;


        public void ReasoningEnter(ChatCompletion completion)
        {
            var message = completion.GetMessage();
            var formattedContent = string.IsNullOrEmpty(_option.ColorHex)
                ? message.ReasoningContent
                : $"<color={_option.ColorHex}>{message.ReasoningContent}";
            AppendContent(formattedContent);
        }

        public void ReasoningUpdate(ChatCompletion completion)
        {
            AppendContent(completion.GetMessage().ReasoningContent);
        }

        public void ReasoningExit(ChatCompletion completion)
        {
            string appendContent;
            if (completion == null)
            {
                appendContent = new string('\n', _option.ReasoningNewlineCount) + "</color>";
            }
            else if (string.IsNullOrEmpty(_option.ColorHex))
            {
                appendContent = new string('\n', _option.ReasoningNewlineCount) + completion.GetMessage().Content;
            }
            else
            {
                appendContent = new string('\n', _option.ReasoningNewlineCount) + "</color>" + completion.GetMessage().ReasoningContent;
            }

            AppendContent(appendContent);
            FlushContentToTextMesh();
        }
    }
}