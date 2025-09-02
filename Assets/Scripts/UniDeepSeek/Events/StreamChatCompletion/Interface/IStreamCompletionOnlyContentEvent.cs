namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IStreamCompletionOnlyContentEvent :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent>
    {
        IStreamCompletionEvent Parent { get; }
    }
}