using System;
using System.Text;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer
{
    public class ContentBuffer : IContentStreamHandler
    {
        public ContentBuffer(Action<string> onFinish, ContentOption option = null, int bufferSize = 512)
        {
            _builder = new StringBuilder(bufferSize);
            _onFinish = onFinish;
            _option = option ?? new ContentOption();
        }

        private readonly StringBuilder _builder;
        private readonly ContentOption _option;
        private readonly Action<string> _onFinish;

        private int _contentLengthCounter;

        public void ContentEnter(ChatCompletion completion)
        {
            AppendContent(completion.GetMessage().Content);
        }

        public void ContentUpdate(ChatCompletion completion)
        {
            AppendContent(completion.GetMessage().Content);
        }

        public void ContentExit(ChatCompletion completion)
        {
            // 确保所有缓冲内容在退出时被刷新
            FlushContentToTextMesh();
        }

        protected void AppendContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            _builder.Append(content);
            _contentLengthCounter += _option.FlushCriteriaOption switch
            {
                ContentFlushCriteriaOption.ByCharacterCount => content.Length,
                ContentFlushCriteriaOption.ByTokenCount => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(_option.FlushCriteriaOption), _option.FlushCriteriaOption, null)
            };

            if (_contentLengthCounter >= _option.FlushThreshold)
                FlushContentToTextMesh();
        }

        protected void FlushContentToTextMesh()
        {
            if (_builder.Length == 0) return;

            _onFinish?.Invoke(_builder.ToString());
            _builder.Clear();
            _contentLengthCounter = 0;
        }
    }
}