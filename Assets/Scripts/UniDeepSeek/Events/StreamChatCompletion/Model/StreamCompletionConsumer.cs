using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public partial class StreamCompletionConsumer : IStreamCompletionConsumer
    {
        private IContentStreamHandler _contentExecuteEvent;
        private IReasoningStreamHandler _reasoningEvent;

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable)
        {
            return StreamCompletionConsumer.ProcessStreamWithReasoningAsync(asyncEnumerable, _contentExecuteEvent, _reasoningEvent);
        }
        

        public void InjectDispatcher(IContentStreamHandler consumer)
        {
            _contentExecuteEvent = consumer;
        }

        public void InjectDispatcher(IReasoningStreamHandler consumer)
        {
            _reasoningEvent = consumer;
        }
    }
}