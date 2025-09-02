using Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public partial class StreamCompletionEventFacade
    {
        public static StreamCompletionConsumer DefaultConsumer { get; } = new();

        public static StreamCompletionEventFacade CreateByDefaultConsumer()
        {
            return new StreamCompletionEventFacade(DefaultConsumer);
        }


        public static StreamCompletionEventFacade CreateByDefaultConsumer(IContentStreamHandler contentStreamHandler)
        {
            var facade = new StreamCompletionEventFacade(DefaultConsumer);
            facade.ContentEvent
                .SetEnter(contentStreamHandler.ContentEnter)
                .SetUpdate(contentStreamHandler.ContentUpdate)
                .SetExit(contentStreamHandler.ContentExit);

            return facade;
        }

        public static StreamCompletionEventFacade CreateByDefaultConsumer(IContentStreamHandler contentStreamHandler,
            IReasoningStreamHandler reasoningStreamHandler)
        {
            var facade = new StreamCompletionEventFacade(DefaultConsumer);
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

        public static StreamCompletionEventFacade CreateByDefaultConsumer(IAggregationStreamHandler aggregationStreamHandler)
        {
            return CreateByDefaultConsumer(aggregationStreamHandler, aggregationStreamHandler);
        }
    }
}