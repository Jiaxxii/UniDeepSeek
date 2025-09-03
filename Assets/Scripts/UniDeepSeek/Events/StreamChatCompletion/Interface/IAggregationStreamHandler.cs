using Xiyu.UniDeepSeek.Events.StreamChatCompletion;

namespace Xiyu.UniDeepSeek.Events
{
    public interface IAggregationStreamHandler : IReasoningStreamHandler, IContentStreamHandler
    {
    }
}