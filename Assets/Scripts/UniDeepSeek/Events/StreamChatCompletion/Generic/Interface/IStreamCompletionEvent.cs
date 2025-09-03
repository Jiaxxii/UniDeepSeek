namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IStreamCompletionEvent<TContext>
    {
        IStreamCompletionOnlyContentEvent<TContext> ContentEvent { get; }
        IStreamCompletionReasoningEvent<TContext> ReasoningEvent { get; }
    }
}