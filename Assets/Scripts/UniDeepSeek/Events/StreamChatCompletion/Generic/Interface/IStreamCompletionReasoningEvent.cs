using System;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IStreamCompletionReasoningEvent<TContext> :
        IEventLifecycleHooks<IStreamCompletionReasoningEvent<TContext>, TContext>
    {
        IStreamCompletionEvent<TContext> Parent { get; }


        IStreamCompletionReasoningEvent<TContext> SetAll(Action<ChatCompletion, TContext> action);
        IStreamCompletionReasoningEvent<TContext> AppendAll(Action<ChatCompletion, TContext> action);
        IStreamCompletionReasoningEvent<TContext> RemoveAllBy(Action<ChatCompletion, TContext> action);
    }
}