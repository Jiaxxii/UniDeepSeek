using System;
using System.Text;
using Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic.Buffer
{
    public class ContentBuffer<TContext> : IContentStreamHandler<TContext>
    {
        public ContentBuffer(Action<string> onFinish, ContentOption? option = null, int bufferSize = 512)
        {
            _builder = new StringBuilder(bufferSize);
            option ??= ContentOption.Default;
            _flushThreshold = option.Value.FlushThreshold;
            _flushCriteriaOption = option.Value.FlushCriteriaOption;
            _contentLengthCounter = 0;
            _onFinish = onFinish;
        }

        private readonly StringBuilder _builder;
        private int _contentLengthCounter;
        private readonly int _flushThreshold;
        private readonly ContentFlushCriteriaOption _flushCriteriaOption;
        private readonly Action<string> _onFinish;

        public void ContentEnter(ChatCompletion completion, TContext context)
        {
            AppendContent(completion.GetMessage().Content);
        }

        public void ContentUpdate(ChatCompletion completion, TContext context)
        {
            AppendContent(completion.GetMessage().Content);
        }

        public void ContentExit(ChatCompletion completion, TContext context)
        {
            // 确保所有缓冲内容在退出时被刷新
            FlushContentToTextMesh();
        }

        protected void AppendContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            _builder.Append(content);
            _contentLengthCounter += _flushCriteriaOption switch
            {
                ContentFlushCriteriaOption.ByCharacterCount => content.Length,
                ContentFlushCriteriaOption.ByTokenCount => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(_flushCriteriaOption), _flushCriteriaOption, null)
            };

            if (_contentLengthCounter >= _flushThreshold)
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