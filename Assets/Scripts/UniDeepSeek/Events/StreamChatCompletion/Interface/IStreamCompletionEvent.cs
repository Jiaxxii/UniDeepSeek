namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IStreamCompletionEvent
    {
        IStreamCompletionOnlyContentEvent ContentEvent { get; }
        IStreamCompletionReasoningEvent ReasoningEvent { get; }
    }
}