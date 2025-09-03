using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public partial class StreamCompletionConsumer<TContext> : IStreamCompletionConsumer<TContext>
    {
        private IContentStreamHandler<TContext> _contentExecuteEvent;
        private IReasoningStreamHandler<TContext> _reasoningEvent;

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context)
        {
            return ProcessStreamWithReasoningAsync(asyncEnumerable, context,_contentExecuteEvent, _reasoningEvent);
        }


        public void InjectDispatcher(IContentStreamHandler<TContext> consumer)
        {
            _contentExecuteEvent = consumer;
        }

        public void InjectDispatcher(IReasoningStreamHandler<TContext> consumer)
        {
            _reasoningEvent = consumer;
        }
    }
}