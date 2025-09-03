namespace Xiyu.UniDeepSeek.Events.Generic
{
    public partial class StreamCompletionEventFacade<TContext> : IStreamCompletionEvent<TContext>
    {
        public StreamCompletionEventFacade(IStreamCompletionConsumer<TContext> consumer)
        {
            ContentEvent = new ContentEvent<TContext>(this);
            ReasoningEvent = new ReasoningEvent<TContext>(this);
            _consumer = consumer;
        }


        public IStreamCompletionOnlyContentEvent<TContext> ContentEvent { get; }
        public IStreamCompletionReasoningEvent<TContext> ReasoningEvent { get; }

        private readonly IStreamCompletionConsumer<TContext> _consumer;


        public IStreamCompletionConsumer<TContext> Builder()
        {
            _consumer.InjectDispatcher((IContentStreamHandler<TContext>)ContentEvent);
            _consumer.InjectDispatcher((IReasoningStreamHandler<TContext>)ReasoningEvent);

            return _consumer;
        }

        public IStreamCompletionConsumer<TContext> BuilderOnlyContent()
        {
            _consumer.InjectDispatcher((IContentStreamHandler<TContext>)ContentEvent);
            _consumer.InjectDispatcher((IReasoningStreamHandler<TContext>)null);


            return _consumer;
        }

        public IStreamCompletionConsumer<TContext> BuilderOnlyReasoning()
        {
            _consumer.InjectDispatcher((IContentStreamHandler<TContext>)null);
            _consumer.InjectDispatcher((IReasoningStreamHandler<TContext>)ReasoningEvent);

            return _consumer;
        }
    }
}