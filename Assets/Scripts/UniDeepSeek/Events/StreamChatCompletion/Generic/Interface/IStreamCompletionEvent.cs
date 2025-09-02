namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IStreamCompletionEvent<TContext>
    {
        IStreamCompletionOnlyContentEvent<TContext> ContentEvent { get; }
        IStreamCompletionReasoningEvent<TContext> ReasoningEvent { get; }
    }
}