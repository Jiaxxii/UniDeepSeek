using System;
using Xiyu.UniDeepSeek.Events.Buffer;

namespace Xiyu.UniDeepSeek.Events.Generic.Buffer
{
    public class ReasoningContentBuffer<TContext> : ContentBuffer<TContext>, IAggregationStreamHandler<TContext>
    {
        public ReasoningContentBuffer(Action<string, TContext> onFinish, ReasoningOption option = null, int bufferSize = 1024)
            : base(onFinish, option, bufferSize)
        {
            _option = option ?? new ReasoningOption();
        }

        private readonly ReasoningOption _option;


        public void ReasoningEnter(ChatCompletion completion, TContext context)
        {
            var message = completion.GetMessage();
            var formattedContent = string.IsNullOrEmpty(_option.ColorHex)
                ? message.ReasoningContent
                : $"<color={_option.ColorHex}>{message.ReasoningContent}";
            AppendContent(formattedContent, context);
        }

        public void ReasoningUpdate(ChatCompletion completion, TContext context)
        {
            AppendContent(completion.GetMessage().ReasoningContent, context);
        }

        public void ReasoningExit(ChatCompletion completion, TContext context)
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

            AppendContent(appendContent, context);
            FlushContentToTextMesh(context);
        }
    }
}