using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public partial class ChatCompletionEvent : IChatCompletionEvent
    {
        public ChatCompletionEvent()
        {
            var chatCompletionContentEvent = new ChatCompletionContentEvent(this);
            var chatCompletionReasoningEvent = new ChatCompletionReasoningEvent(this);

            ContentEventSetting = chatCompletionContentEvent;
            ReasoningEventSetting = chatCompletionReasoningEvent;

            _displayChatStreamProcessor = new DisplayChatStreamProcessor(chatCompletionContentEvent, chatCompletionReasoningEvent);
        }

        public ChatCompletionEvent(IChatCompletionRunning displayChatStreamProcessor)
        {
            ContentEventSetting = new ChatCompletionContentEvent(this);
            ReasoningEventSetting = new ChatCompletionReasoningEvent(this);
            _displayChatStreamProcessor = displayChatStreamProcessor;
        }

        public ChatCompletionEvent(IChatCompletionContentEvent contentEvent, IChatCompletionReasoningEvent reasoningEvent, IChatCompletionRunning displayChatStreamProcessor)
        {
            ContentEventSetting = contentEvent;
            ReasoningEventSetting = reasoningEvent;
            _displayChatStreamProcessor = displayChatStreamProcessor;
        }

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable)
        {
            return _displayChatStreamProcessor.DisplayChatStreamAsync(asyncEnumerable);
        }

        public IChatCompletionContentEvent ContentEventSetting { get; }
        public IChatCompletionReasoningEvent ReasoningEventSetting { get; }

        private readonly IChatCompletionRunning _displayChatStreamProcessor;
    }
}