using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public partial class ChatCompletionEvent<TContext> : IChatCompletionEvent<TContext>
    {
        public ChatCompletionEvent()
        {
            var chatCompletionContentEvent = new ChatCompletionContentEvent<TContext>(this);
            var chatCompletionReasoningEvent = new ChatCompletionReasoningEvent<TContext>(this);

            ContentEventSetting = chatCompletionContentEvent;
            ReasoningEventSetting = chatCompletionReasoningEvent;

            _displayChatStreamProcessor = new DisplayChatStreamProcessor(chatCompletionContentEvent, chatCompletionReasoningEvent);
        }

        public ChatCompletionEvent(IChatCompletionRunning<TContext> displayChatStreamProcessor)
        {
            ContentEventSetting = new ChatCompletionContentEvent<TContext>(this);
            ReasoningEventSetting = new ChatCompletionReasoningEvent<TContext>(this);
            _displayChatStreamProcessor = displayChatStreamProcessor;
        }

        public ChatCompletionEvent(IChatCompletionContentEvent<TContext> contentEvent, IChatCompletionReasoningEvent<TContext> reasoningEvent,
            IChatCompletionRunning<TContext> displayChatStreamProcessor)
        {
            ContentEventSetting = contentEvent;
            ReasoningEventSetting = reasoningEvent;
            _displayChatStreamProcessor = displayChatStreamProcessor;
        }

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context)
        {
            return _displayChatStreamProcessor.DisplayChatStreamAsync(asyncEnumerable, context);
        }

        public IChatCompletionContentEvent<TContext> ContentEventSetting { get; }
        public IChatCompletionReasoningEvent<TContext> ReasoningEventSetting { get; }

        private readonly IChatCompletionRunning<TContext> _displayChatStreamProcessor;
    }
}