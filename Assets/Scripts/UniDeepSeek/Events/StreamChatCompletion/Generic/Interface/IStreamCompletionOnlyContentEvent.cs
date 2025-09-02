namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IStreamCompletionOnlyContentEvent<TContext> :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent<TContext>, TContext>
    {
        IStreamCompletionEvent<TContext> Parent { get; }
    }
}