namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IAggregationStreamHandler<in TContext> :
        IReasoningStreamHandler<TContext>, IContentStreamHandler<TContext>
    {
    }
}