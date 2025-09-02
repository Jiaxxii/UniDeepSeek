using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IStreamCompletionEvent<TContext>
    {
        IStreamCompletionOnlyContentEvent<TContext> ContentEvent { get; }
        IStreamCompletionReasoningEvent<TContext> ReasoningEvent { get; }
    }

    public interface IStreamCompletionOnlyContentEvent<TContext> :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent<TContext>, TContext>
    {
        IStreamCompletionEvent<TContext> Parent { get; }
    }


    public interface IStreamCompletionReasoningEvent<TContext> :
        IEventLifecycleHooks<IStreamCompletionReasoningEvent<TContext>, TContext>
    {
        IStreamCompletionEvent<TContext> Parent { get; }


        IStreamCompletionReasoningEvent<TContext> SetAll(Action<ChatCompletion, TContext> action);
        IStreamCompletionReasoningEvent<TContext> AppendAll(Action<ChatCompletion, TContext> action);
        IStreamCompletionReasoningEvent<TContext> RemoveAllBy(Action<ChatCompletion, TContext> action);
    }
}