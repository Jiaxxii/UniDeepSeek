using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IStreamCompletionEvent
    {
        IStreamCompletionOnlyContentEvent ContentEvent { get; }
        IStreamCompletionReasoningEvent ReasoningEvent { get; }
    }

    public interface IStreamCompletionOnlyContentEvent :
        IEventLifecycleHooks<IStreamCompletionOnlyContentEvent>
    {
        IStreamCompletionEvent Parent { get; }
    }


    public interface IStreamCompletionReasoningEvent :
        IEventLifecycleHooks<IStreamCompletionReasoningEvent>
    {
        IStreamCompletionEvent Parent { get; }


        IStreamCompletionReasoningEvent SetAll(Action<ChatCompletion> action);
        IStreamCompletionReasoningEvent AppendAll(Action<ChatCompletion> action);
        IStreamCompletionReasoningEvent RemoveAllBy(Action<ChatCompletion> action);
    }
}