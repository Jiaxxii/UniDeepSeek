namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IAggregationStreamHandler<in TContext> :
        IReasoningStreamHandler<TContext>, IContentStreamHandler<TContext>
    {
    }
}