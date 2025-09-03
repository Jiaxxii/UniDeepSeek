namespace Xiyu.UniDeepSeek.Events
{
    public interface IStreamCompletionEvent
    {
        IStreamCompletionOnlyContentEvent ContentEvent { get; }
        IStreamCompletionReasoningEvent ReasoningEvent { get; }
    }
}