namespace Xiyu.UniDeepSeek.Events
{
    public interface IStreamCompletionOnlyContentEvent :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent>
    {
        IStreamCompletionEvent Parent { get; }
    }
}