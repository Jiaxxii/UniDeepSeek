namespace Xiyu.UniDeepSeek.Events.Generic
{
    public partial class StreamCompletionEventFacade<TContext>
    {
        public static StreamCompletionConsumer<TContext> DefaultConsumer { get; } = new();

        public static StreamCompletionEventFacade<TContext> CreateByDefaultConsumer()
        {
            return new StreamCompletionEventFacade<TContext>(DefaultConsumer);
        }

        public static StreamCompletionEventFacade<TContext> CreateByDefaultConsumer(IContentStreamHandler<TContext> contentStreamHandler)
        {
            var facade = new StreamCompletionEventFacade<TContext>(DefaultConsumer);
            facade.ContentEvent
                .SetEnter(contentStreamHandler.ContentEnter)
                .SetUpdate(contentStreamHandler.ContentUpdate)
                .SetExit(contentStreamHandler.ContentExit);

            return facade;
        }

        public static StreamCompletionEventFacade<TContext> CreateByDefaultConsumer(IContentStreamHandler<TContext> contentStreamHandler,
            IReasoningStreamHandler<TContext> reasoningStreamHandler)
        {
            var facade = new StreamCompletionEventFacade<TContext>(DefaultConsumer);
            facade.ContentEvent
                .SetEnter(contentStreamHandler.ContentEnter)
                .SetUpdate(contentStreamHandler.ContentUpdate)
                .SetExit(contentStreamHandler.ContentExit);

            facade.ReasoningEvent
                .SetEnter(reasoningStreamHandler.ReasoningEnter)
                .SetUpdate(reasoningStreamHandler.ReasoningUpdate)
                .SetExit(reasoningStreamHandler.ReasoningExit);

            return facade;
        }

        public static StreamCompletionEventFacade<TContext> CreateByDefaultConsumer(IAggregationStreamHandler<TContext> aggregationStreamHandler)
        {
            return CreateByDefaultConsumer(aggregationStreamHandler, aggregationStreamHandler);
        }
    }
}