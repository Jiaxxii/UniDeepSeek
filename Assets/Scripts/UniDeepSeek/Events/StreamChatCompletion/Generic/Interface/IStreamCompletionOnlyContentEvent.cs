namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IStreamCompletionOnlyContentEvent<TContext> :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent<TContext>, TContext>
    {
        IStreamCompletionEvent<TContext> Parent { get; }
    }
}