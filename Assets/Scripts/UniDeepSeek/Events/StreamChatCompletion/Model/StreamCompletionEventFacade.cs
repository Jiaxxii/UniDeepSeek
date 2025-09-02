namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public partial class StreamCompletionEventFacade : IStreamCompletionEvent
    {
        public StreamCompletionEventFacade(IStreamCompletionConsumer consumer)
        {
            ContentEvent = new ContentEvent(this);
            ReasoningEvent = new ReasoningEvent(this);
            _consumer = consumer;
        }


        public IStreamCompletionOnlyContentEvent ContentEvent { get; }
        public IStreamCompletionReasoningEvent ReasoningEvent { get; }

        private readonly IStreamCompletionConsumer _consumer;


        public IStreamCompletionConsumer Builder()
        {
            _consumer.InjectDispatcher((IContentStreamHandler)ContentEvent);
            _consumer.InjectDispatcher((IReasoningStreamHandler)ReasoningEvent);

            return _consumer;
        }

        public IStreamCompletionConsumer BuilderOnlyContent()
        {
            _consumer.InjectDispatcher((IContentStreamHandler)ContentEvent);
            _consumer.InjectDispatcher((IReasoningStreamHandler)null);


            return _consumer;
        }

        public IStreamCompletionConsumer BuilderOnlyReasoning()
        {
            _consumer.InjectDispatcher((IContentStreamHandler)null);
            _consumer.InjectDispatcher((IReasoningStreamHandler)ReasoningEvent);

            return _consumer;
        }
    }
}