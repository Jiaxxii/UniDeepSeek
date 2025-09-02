using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer
{
    public class ReasoningContentBuffer : ContentBuffer, IAggregationStreamHandler
    {
        public ReasoningContentBuffer(Action<string> onFinish, ReasoningOption? option, int bufferSize = 1024)
            : base(onFinish, option, bufferSize)
        {
            option ??= ReasoningOption.Default;
            _colorHex = option.Value.ColorHex;
            _reasoningNewlineCount = option.Value.ReasoningNewlineCount;
        }

        private readonly string _colorHex;
        private readonly int _reasoningNewlineCount;

        public void ReasoningEnter(ChatCompletion completion)
        {
            var message = completion.GetMessage();
            var formattedContent = string.IsNullOrEmpty(_colorHex)
                ? message.ReasoningContent
                : $"<color={(_colorHex.StartsWith('#') ? _colorHex : $"#{_colorHex}")}>{message.ReasoningContent}";
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
                appendContent = new string('\n', _reasoningNewlineCount) + "</color>";
            }
            else if (string.IsNullOrEmpty(_colorHex))
            {
                appendContent = new string('\n', _reasoningNewlineCount) + completion.GetMessage().Content;
            }
            else
            {
                appendContent = new string('\n', _reasoningNewlineCount) + "</color>" + completion.GetMessage().ReasoningContent;
            }

            AppendContent(appendContent);
            FlushContentToTextMesh();
        }
    }
}