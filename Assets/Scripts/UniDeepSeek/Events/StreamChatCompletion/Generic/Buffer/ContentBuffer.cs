using System;
using System.Text;
using Xiyu.UniDeepSeek.Events.Buffer;

namespace Xiyu.UniDeepSeek.Events.Generic.Buffer
{
    public class ContentBuffer<TContext> : IContentStreamHandler<TContext>
    {
        public ContentBuffer(Action<string, TContext> onFinish, ContentOption option = null, int bufferSize = 512)
        {
            _builder = new StringBuilder(bufferSize);
            _onFinish = onFinish;
            _option = option ?? new ContentOption();
        }

        private readonly ContentOption _option;
        private readonly StringBuilder _builder;
        private readonly Action<string, TContext> _onFinish;

        private int _contentLengthCounter;

        public void ContentEnter(ChatCompletion completion, TContext context)
        {
            AppendContent(completion.GetMessage().Content, context);
        }

        public void ContentUpdate(ChatCompletion completion, TContext context)
        {
            AppendContent(completion.GetMessage().Content, context);
        }

        public void ContentExit(ChatCompletion completion, TContext context)
        {
            // 确保所有缓冲内容在退出时被刷新
            FlushContentToTextMesh(context);
        }

        protected void AppendContent(string content, TContext context)
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
                FlushContentToTextMesh(context);
        }

        protected void FlushContentToTextMesh(TContext context)
        {
            if (_builder.Length == 0) return;

            _onFinish?.Invoke(_builder.ToString(), context);
            _builder.Clear();
            _contentLengthCounter = 0;
        }
    }
}